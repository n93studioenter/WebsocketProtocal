using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsocketProtocal
{
    public class ObjDeviceCoordinate
    {
        public Nullable<double> DeviceCoorX { get; set; }
        public Nullable<double> DeviceCoorY { get; set; }
    }
    public static class Helper
	{
        public static ObjDeviceCoordinate getDeviceLocation(float distanceNode1, float distanceNode2, float distanceNode3, float Node1X, float Node2X, float Node3X, float Node1Y, float Node2Y, float Node3Y)
        {
            float ptA, ptB, ptC, ptD, ptE, ptF;
            float CE, FB, EA, BD, CD, AF, AE;
            float resultCoorX, resultCoorY;
            float distanceN1BP = float.Parse(Math.Pow((distanceNode1 / 1000), 2).ToString());
            float distanceN2BP = float.Parse(Math.Pow((distanceNode2 / 1000), 2).ToString());
            float distanceN3BP = float.Parse(Math.Pow((distanceNode3 / 1000), 2).ToString());
            float node1XBP = float.Parse(Math.Pow(Node1X, 2).ToString());
            float node2XBP = float.Parse(Math.Pow(Node2X, 2).ToString());
            float node3XBP = float.Parse(Math.Pow(Node3X, 2).ToString());
            float node1YBP = float.Parse(Math.Pow(Node1Y, 2).ToString());
            float node2YBP = float.Parse(Math.Pow(Node2Y, 2).ToString());
            float node3YBP = float.Parse(Math.Pow(Node3Y, 2).ToString());

            //gán biến thu gọn
            ptA = (-2 * Node1X) + (2 * Node2X);
            ptB = (-2 * Node1Y) + (2 * Node2Y);
            ptC = distanceN1BP - distanceN2BP - node1XBP + node2XBP - node1YBP + node2YBP;
            ptD = (-2 * Node2X) + (2 * Node3X);
            ptE = (-2 * Node2Y) + (2 * Node3Y);
            ptF = distanceN2BP - distanceN3BP - node2XBP + node3XBP - node2YBP + node3YBP;

            CE = ptC * ptE;
            FB = ptF * ptB;
            float CEFB = CE - FB;
            EA = ptE * ptA;
            BD = ptB * ptD;
            float EABD = EA - BD;
            CD = ptC * ptD;
            AF = ptA * ptF;
            float CDAF = CD - AF;
            BD = ptB * ptD;
            AE = ptA * ptE;
            float BDAE = BD - AE;

            //tính toán ra kết quả tọa độ X và Y
            resultCoorX = CEFB / EABD;
            resultCoorY = CDAF / BDAE;

            ObjDeviceCoordinate ObjDevice = new ObjDeviceCoordinate();
            ObjDevice.DeviceCoorX = resultCoorX;
            ObjDevice.DeviceCoorY = resultCoorY;

            return ObjDevice;
        }
    }
} 