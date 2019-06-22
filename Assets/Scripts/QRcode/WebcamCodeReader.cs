using ZXing;
using ZXing.QrCode;
using UnityEngine;
using UnityEngine.UI;

public class WebcamCodeReader {
    private Color32[] data = null;
    private BarcodeReader reader = new BarcodeReader();

    public string Read(WebCamTexture webCamTexture)
    {
        int width = webCamTexture.width;
        int height = webCamTexture.height;

        // 初回だけdataを確保
        if (data == null)
        {
            Debug.Log("init data");
            data = new Color32[width * height];
        }
        webCamTexture.GetPixels32(data);

        Result result = reader.Decode(data, width, height);

        if (result != null)
        {
            return result.Text;
        }
        return null; //エラー時
    }
}
