using System.Collections;
using ZXing;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class WebcamFastCodeReader {
    //private Color32[] data = null;
    private BarcodeReader reader = new BarcodeReader();
    
    public string resultText;
    private int width;
    private int height;
    private Color32[] _buffer;
    private Thread readerThread;
    
    public void SetBuffer(Color32[] buffer)
    {
        if (_buffer == null)
        {
            _buffer = buffer;
        }
    }

    public void StartRead(int w, int h)
    {
        //Debug.Log("start read");

        width = w;
        height = h;
        _buffer = null;
        resultText = null;
        readerThread = new Thread(Read);
        readerThread.Start();
    }

    private void Read()
    {
        while (true)
        {
            if (_buffer == null)
            {
                //Debug.Log("wait read");
                Thread.Sleep(20);
                continue;
            }

            //Debug.Log("reading");

            Result result = reader.Decode(_buffer, width, height);

            if (result != null)
            {
                //Debug.Log("read completed");

                resultText = result.Text;
                break;
            }
            else
            {
                resultText = null; //エラー時
            }

            Thread.Sleep(200);
            _buffer = null;
        }
    }
}
