using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CameraExample : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Don't attempt to use the camera if it is already open
            if (NativeCamera.IsCameraBusy())
                return;

            if (Input.mousePosition.x < Screen.width / 2 && Input.mousePosition.y >Screen.width/2)
            {
                // Take a picture with the camera
                // If the captured image's width and/or height is greater than 512px, down-scale it
                TakePicture(512);
            }
            else if (Input.mousePosition.x > Screen.width / 2 && Input.mousePosition.y > Screen.width / 2)
            {
                // Record a video with the camera
                RecordVideo();
            }
        }
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create a Texture2D from the captured image
                Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //////////////////Add for Cloud Vision API////////////////////
                Texture2D visionTexture = Texture2DFromFile(path);
                visionTexture.ReadPixels(new Rect(0, 0, maxSize, maxSize), 0, 0);
                byte[] jpg = visionTexture.EncodeToJPG();
                string encode = Convert.ToBase64String(jpg);

                StartCoroutine(requestVisionAPI(encode));

                //////////////////////////////////////////////////////////////////

                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.AddComponent<MeshCollider>();
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;

                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
            
        }, maxSize);

        Debug.Log("Permission result: " + permission);
    }

    private void RecordVideo()
    {
        NativeCamera.Permission permission = NativeCamera.RecordVideo((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the recorded video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        });

        Debug.Log("Permission result: " + permission);
    }

    //////////////////Add for Cloud Vision API////////////////////
    public Texture2D Texture2DFromFile(string path)
    {
        Texture2D texture = null;
        if (File.Exists(path))
        {
            //byte取得
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader bin = new BinaryReader(fileStream);
            byte[] readBinary = bin.ReadBytes((int)bin.BaseStream.Length);
            bin.Close();
            fileStream.Dispose();
            fileStream = null;
            if (readBinary != null)
            {
                //横サイズ
                int pos = 16;
                int width = 0;
                for (int i = 0; i < 4; i++)
                {
                    width = width * 256 + readBinary[pos++];
                }
                //縦サイズ
                int height = 0;
                for (int i = 0; i < 4; i++)
                {
                    height = height * 256 + readBinary[pos++];
                }
                //byteからTexture2D作成

                texture = new Texture2D(512, 512);
                texture.LoadImage(readBinary);
            }
            readBinary = null;
        }
        return texture;
    }

    private IEnumerator requestVisionAPI(string base64Image)
    {
        Debug.Log("VisionAPICalled");
        string apiKey = "AIzaSyC_HYRYsT_fw1KO8VeSR7xJz_UzPP5rCYc";
        string url = "https://vision.googleapis.com/v1/images:annotate?key=" + apiKey;

        // requestBodyを作成
        var requests = new requestBody();
        requests.requests = new List<AnnotateImageRequest>();

        var request = new AnnotateImageRequest();
        request.image = new Image();
        request.image.content = base64Image;

        request.features = new List<Feature>();
        var feature = new Feature();
        feature.type = FeatureType.LABEL_DETECTION.ToString();
        feature.maxResults = 10;
        request.features.Add(feature);



        requests.requests.Add(request);

        // JSONに変換
        string jsonRequestBody = JsonUtility.ToJson(requests);

        // ヘッダを"application/json"にして投げる
        var webRequest = new UnityWebRequest(url, "POST");
        byte[] postData = Encoding.UTF8.GetBytes(jsonRequestBody);
        webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(postData);
        webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");

        yield return webRequest.Send();

        if (webRequest.isNetworkError)
        {
            // エラー時の処理
            Debug.Log(webRequest.error);
        }
        else
        {
            // 成功時の処理
            Debug.Log(webRequest.downloadHandler.text);
            var responses = JsonUtility.FromJson<responseBody>(webRequest.downloadHandler.text);

        }
    }

    [Serializable]
    public class requestBody
    {
        public List<AnnotateImageRequest> requests;
    }

    [Serializable]
    public class AnnotateImageRequest
    {
        public Image image;
        public List<Feature> features;
        //public string imageContext;
    }

    [Serializable]
    public class Image
    {
        public string content;
        //public ImageSource source;
    }

    [Serializable]
    public class ImageSource
    {
        public string gcsImageUri;
    }

    [Serializable]
    public class Feature
    {
        public string type;
        public int maxResults;
    }

    public enum FeatureType
    {
        TYPE_UNSPECIFIED,
        FACE_DETECTION,
        LANDMARK_DETECTION,
        LOGO_DETECTION,
        LABEL_DETECTION,
        TEXT_DETECTION,
        SAFE_SEARCH_DETECTION,
        IMAGE_PROPERTIES
    }

    [Serializable]
    public class ImageContext
    {
        public LatLongRect latLongRect;
        public string languageHints;
    }

    [Serializable]
    public class LatLongRect
    {
        public LatLng minLatLng;
        public LatLng maxLatLng;
    }

    [Serializable]
    public class LatLng
    {
        public float latitude;
        public float longitude;
    }

    [Serializable]
    public class responseBody
    {
        public List<AnnotateImageResponse> responses;
    }

    [Serializable]
    public class AnnotateImageResponse
    {
        public List<EntityAnnotation> labelAnnotations;
    }

    [Serializable]
    public class EntityAnnotation
    {
        public string mid;
        public string locale;
        public string description;
        public float score;
        public float confidence;
        public float topicality;
        public BoundingPoly boundingPoly;
        public List<LocationInfo> locations;
        public List<Property> properties;
    }

    [Serializable]
    public class BoundingPoly
    {
        public List<Vertex> vertices;
    }

    [Serializable]
    public class Vertex
    {
        public float x;
        public float y;
    }

    [Serializable]
    public class LocationInfo
    {
        LatLng latLng;
    }

    [Serializable]
    public class Property
    {
        string name;
        string value;
    }



}