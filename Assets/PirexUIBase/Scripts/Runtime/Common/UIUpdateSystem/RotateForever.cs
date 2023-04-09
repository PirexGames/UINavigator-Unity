using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pirex.UI.Base
{
    public class RotateForever : UIUpdateComp
    {
        [SerializeField] private float speed = 180f;
        public override void OnUpdate()
        {
            transform.Rotate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
