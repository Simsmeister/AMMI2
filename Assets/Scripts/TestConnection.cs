using Unity.Engine;
using System.Collections.Generic;
using System.IO.Ports;

public class TestConnection : MonoBehaviour
{
    SerialPort data_stream = new SerialPort("/dev/cu.usbmodem101", 9600);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
