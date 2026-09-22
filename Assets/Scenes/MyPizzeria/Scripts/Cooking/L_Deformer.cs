using LyviN.Player.Baking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.kampe1234.Player.Baking
{
    public class L_Deformer : MonoBehaviour
    {
        public Dough Me;
        public L_DeformableMesh deformableMesh;
        public string RequiredTag;
        public float collisionRadius = 0.3f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionStay(Collision collision)
        {
            if (BaseBake.Instance != null && BaseBake.Instance.editingDough == Me && collision.transform.parent == null)
            {
                if (collision.transform.tag == RequiredTag)
                {
                    Debug.Log(1);
                    foreach (var contact in collision.contacts)
                    {
                        deformableMesh.AddDepression(transform.InverseTransformPoint(contact.point), collisionRadius);
                    }

                }
                else
                {
                    Debug.Log(collision.transform.tag);
                }
            }
        }

    }
}