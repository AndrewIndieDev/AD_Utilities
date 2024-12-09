//using Unity.Netcode;
using Unity.Netcode.Components;
//using UnityEngine;

namespace AndrewDowsett.Networking
{
    public class ClientNetworkTransform : NetworkTransform
    {
        //private Vector3 previousPosition;

        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }

        private void Start()
        {
            //if (IsServer)
            //{
            //    OnClientRequestChange += ClientPrediction;
            //}
        }

        //private float GetMagnitude(Vector3 a, Vector3 b)
        //{
        //    float aMag = GetMagnitude(a);
        //    float bMag = GetMagnitude(b);
        //    return Mathf.Abs(aMag - bMag);
        //}

        //private float GetMagnitude(Vector3 a)
        //{
        //    return Mathf.Sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z));
        //}

        //private (Vector3 pos, Quaternion rotOut, Vector3 scale) ClientPrediction(Vector3 pos, Quaternion rot, Vector3 scale)
        //{
        //    Debug.Log(
        //        $"Old Position: {transform.position} | Old Magnitutde: {GetMagnitude(transform.position)}\n" +
        //        $"New Position: {pos} | New Magnitude: {GetMagnitude(pos)}\n" +
        //        $"Magnitutde of move: {GetMagnitude(transform.position, pos)}");

        //    previousPosition = transform.position;
        //    transform.position = pos;
        //    transform.rotation = rot;
        //    transform.localScale = scale;
        //    return (pos, rot, scale);
        //}
    }
}