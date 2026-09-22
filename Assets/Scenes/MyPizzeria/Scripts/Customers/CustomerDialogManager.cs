using LyviN.Player.Control;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LyviN.AI.Customers
{
    
    public class CustomerDialogManager : MonoBehaviour
    {
        public CustomerMain myCustomer;
        public float LetterDelay = 0.08f;
        public float TextHideTime = 3f;
        public GameObject myDialogObject;
        public GameObject myDialogChatbox;
        public TextMeshPro myDialogText;
        public AudioSource myDialogAudio;
        public bool TypeRandomBullshit;
        private void Update()
        {
            if (TypeRandomBullshit)
            {
                TypeRandomBullshit = false;
                Type($"{Shared.RandomString(5)} {Shared.RandomString(10)} {Shared.RandomString(7)}");
            }
            if (myDialogAudio == null && myCustomer.mySkin != null)
            {
                myDialogAudio = myCustomer.mySkin.GetComponent<AudioSource>();
            }
        }
        public bool isEmpty()
        {
            return myDialogText.text == "" || myDialogText.text == "<>";
        }
        private void Start()
        {
            myDialogObject.SetActive(false);
        }
        public void Type(string text, bool needsClosePlayer = false)
        {
            StartCoroutine(ProgressiveTyping(text, needsClosePlayer));
        }
        public IEnumerator ProgressiveTyping(string text, bool needsClosePlayer)
        {
            CancelInvoke("HideText");
            myDialogObject.SetActive(true);
            myDialogText.text = "";
            int i = 0;
            char[] t = text.ToCharArray();
            Transform player = FirstPersonCamera.Instance.transform;
            while (myDialogText.text != text)
            {
                while (needsClosePlayer && myDialogText.text != "" && Vector3.Distance(player.position, transform.position) > 9f)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (t.Length >= i + 1)
                {
                    myDialogText.text = $"{myDialogText.text}{t[i]}";
                    if (myDialogAudio != null)
                    {
                        myDialogAudio.Play();
                    }
                    if (t[i] == '.')
                    {
                        yield return new WaitForSeconds(0.6f);
                    }
                    else if (t[i] == ',')
                    {
                        yield return new WaitForSeconds(0.3f);

                    }
                    else if (t[i] == '\n')
                    {
                        yield return new WaitForSeconds(1f);
                    }
                    i++;
                }
                yield return new WaitForSeconds(LetterDelay);
            }
            Invoke("HideText", TextHideTime);
            yield return null;

        }
        void HideText()
        {
            CancelInvoke("HideText");
            myDialogText.text = "";
            myDialogObject.SetActive(false);
        }
    }
}