using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarcodeReaderThreadManager : MonoBehaviour {
    public Text resultText; //読み取り結果
    public RawImage cameraPanel;

    public Button btnRegist;
    public Button btnCancel;

    private WebCamTexture webcamTexture;
    private WebcamFastCodeReader reader;

    private int width = 640;
    private int height = 480;
    private string foundString = null;
    private int door_state;
    private string[] ID;

    private SerialPortWrapper _serialPort;

    Firebase fb = new Firebase();

    void Awake()
    {
        _serialPort = new SerialPortWrapper("COM6", 9600);
    }

    private void OnDisable()
    {
        _serialPort.KillThread();
    }

    private IEnumerator Start()
    {
        // ボタンは最初は非表示
        changeBtnVisible(false);

        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogFormat("カメラがありません。");
            yield break;
        }

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogFormat("カメラ利用が許可されていません。");
            yield break;
        }

        WebCamDevice userCameraDevice = WebCamTexture.devices[0];

        webcamTexture = new WebCamTexture(userCameraDevice.name, width, height);
        //webcamTexture = new WebCamTexture(userCameraDevice.name);

        cameraPanel.texture = webcamTexture;
        webcamTexture.Play();

        Debug.Log(webcamTexture.width + " " + webcamTexture.height + " " + webcamTexture.requestedFPS);

        // バーコードリーダーのセット
        this.reader = new WebcamFastCodeReader();

        if(webcamTexture != null)
        {
            this.reader.StartRead(webcamTexture.width, webcamTexture.height);
        }
    }

    System.Action<string> test1 = (text) =>
    {
        Debug.Log(text);
        
    };

    System.Action<bool> test2 = (text) =>
    {
        Debug.Log(text);
    };

    // Update is called once per frame
    void Update()
    {
        //OnGUI();
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (webcamTexture == null || !webcamTexture.isPlaying)
        {
            return;
        }
        
        foundString = this.reader.resultText;

        if (foundString == null)
        {
            resultText.text = "<color=#111111>scanning...</color>";

            // コード発見前
            // バーコード認識

            Color32[] buffer = webcamTexture.GetPixels32();
            this.reader.SetBuffer(buffer);
        }
        else
        {
            //コード発見後
            if(door_state == 0){
                ID = foundString.Split(',');
                Debug.Log(ID[0]);
                Debug.Log(ID[1]);
                door_state = 2;
                _serialPort.Write("n");
            }
            else if(door_state == 1){
                ID = foundString.Split(',');
                Debug.Log(ID[0]);
                Debug.Log(ID[1]);
                _serialPort.Write("f");
            }
            
            resultText.text = "<color=#111111>" + foundString + "</color>";
            Debug.Log(resultText.text);
            changeBtnVisible(true);
            
        }
    }

    void changeBtnVisible(bool state)
    {
        btnRegist.gameObject.SetActive(state);
        btnCancel.gameObject.SetActive(state);
    }

    private void openBarcode(string str)
    {
        if (str.StartsWith("http"))
        {
            Application.OpenURL(str);
        }
        else
        {
            Application.OpenURL("http://www.google.com/search?q=" + str);
        }
    }


    public void onBtnRegistClick()
    {
        StartCoroutine(fb.CloseBox(ID[0], ID[1], test1));
        //openBarcode(foundString);
    }

    public void onBtnCancelClick()
    {
        StartCoroutine(fb.OpenBox(ID[0], ID[1], test2));
        door_state = 1; 
        
        changeBtnVisible(false);
        foundString = null;
        this.reader.StartRead(webcamTexture.width, webcamTexture.height); 
    }
}
