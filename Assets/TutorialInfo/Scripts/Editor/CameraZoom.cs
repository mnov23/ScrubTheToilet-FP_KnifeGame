using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TutorialInfo.Scripts.Editor
{
    public class CameraZoom : MonoBehaviour
    {
        public float zoomSpeed = 10f;
        public float minFOV = 30f;
        public float maxFOV = 60f;
        private Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * zoomSpeed, minFOV, maxFOV);
        }
    }
}
