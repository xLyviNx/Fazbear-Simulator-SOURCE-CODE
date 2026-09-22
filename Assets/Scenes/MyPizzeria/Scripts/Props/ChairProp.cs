using LyviN.AI.Customers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Map.Props
{
    public class ChairProp : MonoBehaviour
    {
        public PlacedProp Prop;
        public TableProp myTable;

        public CustomerMain currentlySitting;
        public bool isTaken()
        {
            return currentlySitting != null;
        }
        public LayerMask tableMask;
        Ray ray;
        public Collider myCollider;
        public void StartFindTable()
        {
            StartCoroutine(FindATable());
        }
        
        IEnumerator FindATable()
        {
            while(transform.position != Prop.Pos)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.1f);
            //Debug.DrawLine(transform.position + (Vector3.up * 0.5f), transform.position + (Vector3.up * 0.5f) + (transform.forward * 1f), Color.cyan, 5f);
            gameObject.layer = 2;
            if (Physics.Raycast(transform.position + (Vector3.up * 0.5f) - (transform.forward * 0.5f),(transform.forward * 1f), out RaycastHit hit, 1f, tableMask))
            {
                if (hit.transform.tag == "PlacedProp")
                {
                    TableProp t = hit.transform.gameObject.GetComponent<TableProp>();
                    if (t != null)
                    {
                        Debug.DrawLine(transform.position + (Vector3.up * 0.5f) - (transform.forward * 0.5f), hit.point, Color.green, 60f);
                        myTable = t;
                        t.myChairs.Add(this);
                    }
                }
            }
            else
            {
                Debug.DrawRay(transform.position + (Vector3.up * 0.5f) - (transform.forward * 0.5f),(transform.forward * 1f), Color.red, 60f);
            }
            gameObject.layer = 9;
            yield return null;
        }
        private void Update()
        {
            if (myCollider == null)
            {
                myCollider = GetComponent<Collider>();
            }
            else
            {
                myCollider.enabled = currentlySitting == null;
            }
        }
        private void OnDestroy()
        {
            if (myTable !=null)
            {
                myTable.myChairs.Remove(this);
            }
        }
    }

}