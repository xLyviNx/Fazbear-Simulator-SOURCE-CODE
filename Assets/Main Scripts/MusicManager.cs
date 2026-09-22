using LyviN.Input;
using LyviN.Map;
using LyviN.Map.Animatronics;
using LyviN.Map.Visiting;
using LyviN.Player;
using LyviN.Player.Baking;
using LyviN.Player.Build;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LyviN.Audio
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        public AudioInfo CatalogueMusic;
        public AudioInfo OrdersMusic;
        public AudioInfo PizzeriaManMusic;
        public AudioInfo BackgroundNormalMusic;
        public AudioInfo BackgroundNightMusic;
        public AudioInfo MenuMusic;
        public AudioInfo BackgroundPOpenMusic;
        public AudioInfo BackgroundOrderMusic;
        public AudioInfo BackgroundBuildMusic;

        public PlacedAnimatronic isCloseToAnimatronic;
        private void Start()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            
            
            if (CursorManager.Instance != null)
            {
                CursorManager c = CursorManager.Instance;
                bool loaded = false;
                if (SaveSystem.Instance != null)
                {
                    loaded = SaveSystem.Instance.Loaded;
                }
                else if (LoadOtherPizzeria.Instance != null)
                {
                    loaded = LoadOtherPizzeria.Instance.Loaded;
                }

                if (loaded || !c.isGameScene)
                {
                    CatalogueMusic.Mute = true;
                    OrdersMusic.Mute = true;
                    PizzeriaManMusic.Mute = true;
                    BackgroundNormalMusic.Mute = true;
                    BackgroundNightMusic.Mute = true;
                    BackgroundPOpenMusic.Mute = true;
                    BackgroundOrderMusic.Mute = true;
                    BackgroundBuildMusic.Mute = true;
                    MenuMusic.Mute = true;

                    if (c.isGameScene)
                    {

                        if (c.isESC)
                        {

                        }
                        else if (c.isCat || c.isStructureInteraction)
                        {
                            CatalogueMusic.Mute = false;
                        }
                        else if (c.isOrders)
                        {
                            OrdersMusic.Mute = false;
                        }
                        else if (c.isPizzeriaManagement)
                        {
                            PizzeriaManMusic.Mute = false;
                        }
                        else
                        {
                            if (TimeSystem.Instance != null)
                            {
                                if (BuildManager.Instance != null && BuildManager.Instance.Building == BuildingType.None)
                                {
                                    if (TimeSystem.Instance.FinalHour >= 6 && TimeSystem.Instance.FinalHour < 22)
                                    {
                                        if (OrdersManager.Instance != null)
                                        {

                                            if (ThePizzeria.Instance.isOpened())
                                            {
                                                if (OrdersManager.Instance.Orders.Count == 0)
                                                {
                                                    if (isCloseToAnimatronic == null)
                                                    {
                                                        BackgroundPOpenMusic.Mute = false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (isCloseToAnimatronic == null)
                                                    {
                                                        BackgroundOrderMusic.Mute = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (isCloseToAnimatronic == null)
                                                {
                                                    BackgroundNormalMusic.Mute = false;
                                                }
                                            }
                                        }
                                        

                                    }
                                    else
                                    {
                                        if (ThePizzeria.Instance != null)
                                        {
                                            if (ThePizzeria.Instance.isOpened())
                                            {
                                                if (OrdersManager.Instance.Orders.Count == 0)
                                                {
                                                    if (isCloseToAnimatronic == null)
                                                    {
                                                        BackgroundPOpenMusic.Mute = false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (isCloseToAnimatronic == null)
                                                    {
                                                        BackgroundOrderMusic.Mute = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (isCloseToAnimatronic == null)
                                                {
                                                    BackgroundNightMusic.Mute = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            BackgroundNightMusic.Mute = false;
                                        }
                                    }
                                }
                                else if (BuildManager.Instance != null)
                                {
                                    if (isCloseToAnimatronic == null)
                                    {
                                        BackgroundBuildMusic.Mute = false;
                                    }
                                }
                                //else
                                //{
                                //if (isCloseToAnimatronic == null)
                                //{
                                //BackgroundNormalMusic.Mute = false;
                                //}
                                //}
                            }
                                
                            
                        }
                    }
                    else
                    {
                        MenuMusic.Mute = false;
                    }
                }
            }
        }
    }
}