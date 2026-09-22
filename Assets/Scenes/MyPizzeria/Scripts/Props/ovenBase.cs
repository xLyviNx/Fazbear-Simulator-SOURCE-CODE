using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LyviN.Player.Baking {

    public class ovenBase : MonoBehaviour
    {
        public GameObject TempKnob;

        public int currTemp;
        public float currTempF;
        public int SelectedTemp;
        public int TargetTemp;
        public Vector3 KnobRot;
        public float KnobSpin;

        public int Timer = -1;

        public Transform CameraTarget;

        public bool isOn;
        public GameObject[] Switches;
        public Light OvenLight;
        public bool isOpen;
        public Animator myAnim;
        public Dough myPizza;
        public bool changingtemp;
        public bool changingtime;
        public GameObject SwitchO;
        public GameObject TimerKnob;
        public Vector3 TimerKnobRot;
        public float TimerKnobSpin;

        public Transform TempCTarget;
        public Transform TimerCTarget;
        public AudioSource switchSound;
        public AudioSource PowerSwitchSound;
        [SerializeField] int lastTemp;
        [SerializeField] int lastTime;
        public GameObject DoorObject;
        public Transform PizzaTarget;
        bool counting;
        bool blockON;
        public AudioSource Alarm;
        public GameObject[] KeypadButtons;
        public List<char> time = new List<char>();
        public TextMeshPro TempText;
        public TextMeshPro TimeText;
        private void Update()
        {
            if (myPizza != null)
            {
                myPizza.transform.position = PizzaTarget.position;
            }
            if (isOpen)
            {
                isOn = false;
            }
            KnobSpin = Mathf.Clamp(KnobSpin, 0, 320f);
            KnobRot = new Vector3(180f, KnobSpin, 0f);
            TempKnob.transform.localEulerAngles = KnobRot;

            Switches[0].SetActive(isOn);
            Switches[1].SetActive(!isOn);
            OvenLight.gameObject.SetActive(isOn && currTemp<TargetTemp);
            SelectedTemp = (int)(KnobSpin * 1.25f);
            if (isOn)
            {
                TargetTemp = SelectedTemp;
            }
            else
            {
                TargetTemp = 0;
            }
            if (TimerKnob != null)
            {
                if (!counting)
                {
                    Timer = (int)System.MathF.Round(TimerKnobSpin * 1.5f);
                }
                else
                {
                    TimerKnobSpin = Timer / 1.5f;
                }
                TimerKnobSpin = Mathf.Clamp(TimerKnobSpin, 0, 320f);
                TimerKnobRot = new Vector3(180f, TimerKnobSpin, 0f);
                TimerKnob.transform.localEulerAngles = TimerKnobRot;
            }
            else if (KeypadButtons.Length > 0)
            {
                if (TempText != null) { TempText.text = $"{currTemp} *C"; }
                string displayt = "--:--";
                if (!counting)
                {
                    if (time.Count > 3)
                    {
                        string secs = time[2].ToString() + time[3].ToString();
                        string mins = $"{time[0].ToString() + time[1].ToString()}";
                        if (int.Parse(secs) > 60)
                        {
                            secs = "60";
                        }
                        Timer = int.Parse(mins) * 60 + int.Parse(secs);
                        displayt = $"{mins}:{secs}";
                    }
                    else if(time.Count > 2)
                    {
                        string secs = time[1].ToString() + time[2].ToString();
                        string mins = $"0{time[0].ToString()}";
                        if (int.Parse(secs) > 60)
                        {
                            secs = "60";
                        }
                        Timer = int.Parse(mins) * 60 + int.Parse(secs);
                        displayt = $"{mins}:{secs}";
                    }
                    else if (time.Count > 1)
                    {
                        string secs = time[0].ToString() + time[1].ToString();
                        if (int.Parse(secs) > 60)
                        {
                            secs = "60";
                        }
                        Timer = int.Parse(secs);
                        displayt = $"00:{secs}";
                    }
                    else if (time.Count > 0)
                    {
                        displayt = $"00:0{time[0].ToString()}";
                        Timer = int.Parse(time[0].ToString());
                    }
                    else
                    {
                        Timer = -1;
                    }
                }
                else
                {
                    int mins = Timer / 60;
                    int secs = Timer - (mins * 60);
                    string md = $"{mins}";
                    if (mins < 10)
                    {
                        md = $"0{mins}";
                    }
                    string sd = $"{secs}";
                    if (secs < 10)
                    {
                        sd = $"0{secs}";
                    }
                    displayt = $"{md}:{sd}";
                }
                TimeText.text = displayt;
            }
            
            if (currTemp < TargetTemp)
            {
                currTempF = Mathf.Clamp(currTempF + Time.deltaTime * 3.5f, 0, 400);
            }
            else if (currTemp > TargetTemp)
            {
                currTempF = Mathf.Clamp(Mathf.Lerp(currTempF, TargetTemp, Time.deltaTime * 0.015f), 0, 400);
            }
            currTemp = (int)currTempF;
            myAnim.SetBool("Opened", isOpen);
            if (BaseBake.Instance != null)
            {
                if (BaseBake.Instance.bakeMode == "oven" && BaseBake.Instance.OvenInteraction == this)
                {
                    if (Physics.Raycast(BaseBake.Instance.BakeCam.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hit))
                    {
                        if (!changingtemp && !changingtime)
                        {
                            if (UnityEngine.Input.GetMouseButtonDown(0))
                            {
                                if (hit.transform.gameObject == TempKnob)
                                {
                                    changingtemp = true;
                                    changingtime = false;
                                }
                                else if (hit.transform.gameObject == TimerKnob)
                                {
                                    changingtime = true;
                                    changingtemp = false;
                                }
                                else if (hit.transform.gameObject == SwitchO && !isOpen && (!blockON || isOn))
                                {
                                    if (Timer != 0 || isOn)
                                    {
                                        counting = false;
                                        PowerSwitchSound.Play();
                                        isOn = !isOn;
                                        if (isOn)
                                        {
                                            StartCoroutine(CountTime());
                                        }
                                    }
                                    else
                                    {
                                        changingtime = true;
                                    }
                                }
                                else if (hit.transform.gameObject == DoorObject)
                                {
                                    isOpen = !isOpen;
                                    if (isOn)
                                    {
                                        if (counting)
                                        {
                                            counting = false;
                                        }
                                        PowerSwitchSound.Play();
                                    }
                                }
                                else if (KeypadButtons.Length > 0 && hit.transform.gameObject.name.Contains("Num"))
                                {
                                    GameObject g = hit.transform.gameObject;
                                    if (g==KeypadButtons[0])
                                    {
                                        AddTimer('0');
                                    }
                                    else if (g == KeypadButtons[1])
                                    {
                                        AddTimer('1');
                                    }
                                    else if (g == KeypadButtons[2])
                                    {
                                        AddTimer('2');
                                    }
                                    else if (g == KeypadButtons[3])
                                    {
                                        AddTimer('3');
                                    }
                                    else if (g == KeypadButtons[4])
                                    {
                                        AddTimer('4');
                                    }
                                    else if (g == KeypadButtons[5])
                                    {
                                        AddTimer('5');
                                    }
                                    else if (g == KeypadButtons[6])
                                    {
                                        AddTimer('6');
                                    }
                                    else if (g == KeypadButtons[7])
                                    {
                                        AddTimer('7');
                                    }
                                    else if (g == KeypadButtons[8])
                                    {
                                        AddTimer('8');
                                    }
                                    else if (g == KeypadButtons[9])
                                    {
                                        AddTimer('9');
                                    }
                                    else if (g == KeypadButtons[10])
                                    {
                                        AddTimer('D');
                                    }
                                    else if (g == KeypadButtons[11])
                                    {
                                        AddTimer('O');
                                    }
                                    
                                }
                                else
                                {
                                    
                                }

                            }
                        }
                        if (UnityEngine.Input.GetMouseButton(0) && changingtemp)
                        {
                            changingtime = false;
                            if (lastTemp != SelectedTemp)
                            {
                                if (!switchSound.isPlaying)
                                {
                                    switchSound.Play();
                                }
                                lastTemp = SelectedTemp;
                            }
                            if (lastTime != Timer)
                            {
                                if (!switchSound.isPlaying)
                                {
                                    switchSound.Play();
                                }
                                lastTime = Timer;
                            }
                            float MouseX = UnityEngine.Input.GetAxis("Mouse X");
                            float MouseY = UnityEngine.Input.GetAxis("Mouse Y");
                            KnobSpin += MouseX * 3f+ MouseY * 3f;

                        }
                        else if (UnityEngine.Input.GetMouseButton(0) && changingtime && !counting)
                        {
                            changingtemp = false;
                            
                            if (lastTime != Timer)
                            {
                                if (!switchSound.isPlaying)
                                {
                                    switchSound.Play();
                                }
                                lastTime = Timer;
                            }
                            float MouseX = UnityEngine.Input.GetAxis("Mouse X");
                            float MouseY = UnityEngine.Input.GetAxis("Mouse Y");
                            TimerKnobSpin += MouseX * 3f + MouseY * 3f;

                        }
                        else
                        {
                            changingtime = false;
                               changingtemp = false;
                        }
                    }
                    
                }
                else
                {
                    changingtemp = false;
                }
            }
        }
        void TurnON()
        {
            if (!isOpen && (!blockON || isOn))
            {
                if (Timer != 0 || isOn)
                {
                    counting = false;
                    PowerSwitchSound.Play();
                    isOn = !isOn;
                    if (isOn)
                    {
                        StartCoroutine(CountTime());
                    }
                }
            }
        }
        void AddTimer(char T)
        {
            if (!isOn && !counting)
            {
                if (T == 'D')
                {
                    if (time.Count > 0)
                    {
                        time.RemoveAt(time.Count - 1);
                    }
                }
                else if (T == 'O')
                {
                    if (time.Count > 0)
                    {
                        if (!isOn)
                        {
                            TurnON();
                        }
                    }
                }
                else
                {
                    if (time.Count < 4)
                    {
                        time.Add(T);
                    }
                    else
                    {
                        time[0] = time[1];
                        time[1] = time[2];
                        time[2] = time[3];
                        time[3] = T;
                    }
                }
                switchSound.Play();
            }
        }

        public IEnumerator CountTime()
        {
            if (!blockON)
            {
                Debug.Log(1);
                if (!counting && Timer > 0)
                {
                    blockON = true;
                    Debug.Log(2);
                    counting = true;
                    while (Timer > 0 && counting)
                    {
                        if (!counting)
                        {
                            blockON = false;
                            yield break;

                        }
                        yield return new WaitForSeconds(1f);
                        if (counting)
                        {
                            Timer--;
                            if (time.Count > 0)
                            {
                                string ct = Timer.ToString();
                                time = new List<char>(ct.ToCharArray());
                            }
                        }else
                        {
                            blockON = false;
                            yield break;
                            
                        }
                    }
                    Debug.Log(4);
                    if (counting)
                    {
                        Debug.Log(5);
                        if (isOn)
                        {
                            isOn = false;
                        }
                        if (!isOpen)
                        {
                            isOpen = true;
                        }
                        counting = false;
                        Timer = -1;
                        Alarm.Play();
                    }
                }
                blockON = false;
                yield return null;
            }
        }
    }
}
