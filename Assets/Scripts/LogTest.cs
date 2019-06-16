using UnityEngine;
using System.Collections;

public class LogTest : MonoBehaviour
{
    private IEnumerator Start()
    {
        while (true)
        {
            Debug.LogFormat("Time:{0}", System.DateTime.Now);
            yield return new WaitForSeconds(1.0f);
        }
    }

} // class TestComponent