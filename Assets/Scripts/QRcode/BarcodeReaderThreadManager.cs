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

    private SerialPortWrapper _serialPort;

    void Awake()
    {
        _serialPort = new SerialPortWrapper("COM6", 9600);
    }

    private void OnDisable()
    {
        _serialPort.KillThread();
    }

    void OnGUI()
    {
        if (GUILayout.Button("n", GUILayout.Width(200f), GUILayout.Height(60f)))
        {
            // 文字列nを送信
            _serialPort.Write("n");
        }

        if (GUILayout.Button("f", GUILayout.Width(200f), GUILayout.Height(60f)))
        {
            // 文字列fを送信
            _serialPort.Write("f");
        }
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
            /*
            string codeResult = this.reader.Read(webcamTexture);
            if (codeResult == null)
            {
                resultText.text = "<color=#111111>scanning...</color>";
            }
            else
            {
                foundString = codeResult;
                resultText.text = "<color=#111111>" + codeResult + "</color>";
            }
            */
            //Debug.Log(": reading");

            Color32[] buffer = webcamTexture.GetPixels32();
            this.reader.SetBuffer(buffer);
        }
        else
        {
            //コード発見後
            resultText.text = "<color=#111111>" + foundString + "</color>";
            _serialPort.Write("n");
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
        openBarcode(foundString);
    }

    public void onBtnCancelClick()
    {
        changeBtnVisible(false);
        foundString = null;
        this.reader.StartRead(webcamTexture.width, webcamTexture.height);
    }
}
