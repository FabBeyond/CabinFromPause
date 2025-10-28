using BepInEx;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CabinFromPause
{
    [BepInPlugin("com.fabbeyond.cabinfrompause", "CabinFromPause", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        string lastCabin = "-";
        LevelEditorManager levelEditorManager;

        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Update()
        {
            
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Peaks_CustomLevel")
                levelEditorManager = GameObject.Find("LevelEditorManager").GetComponent<LevelEditorManager>();
            if (scene.name == "Cabin") lastCabin = "MainCabin";
            else if (scene.name == "Category4_1_Cabin") lastCabin = "NorthernCabin";
            else if (scene.name == "Alps_Main") lastCabin = "AlpsCabin";
            else if (scene.name == "Peaks_CustomLevel_Staging") lastCabin = "EditorCabin";
            else if (scene.name == "TitleScreen") ;
            else
            {
                GameObject quitButton = GameObject.Find("Canvas/InGameMenu/InGameMenuObj_DisableMe/menu_pg/MenuContainer/Quit");
                GameObject cabinButton = Instantiate(quitButton, quitButton.transform.parent);
                cabinButton.name = "Cabin";
                cabinButton.transform.SetSiblingIndex(3);
                GameObject.Find("Canvas/InGameMenu/InGameMenuObj_DisableMe/menu_pg/MenuContainer/Cabin/Text").GetComponent<Text>().text = "Cabin";
                UnityEngine.UI.Button cabinButtonBtn = cabinButton.GetComponent<UnityEngine.UI.Button>();

                cabinButtonBtn.onClick.RemoveAllListeners();
                cabinButtonBtn.onClick.AddListener(GoToCabin);
            }
        }

        void GoToCabin()
        {
            if (lastCabin != "-")
            {
                if (lastCabin == "MainCabin")
                {
                    GameManager.control.Save();
                    SceneManager.LoadSceneAsync("Cabin");

                }
                else if (lastCabin == "NorthernCabin")
                {
                    GameManager.control.Save();
                    SceneManager.LoadSceneAsync("Category4_1_Cabin");
                }
                else if (lastCabin == "AlpsCabin")
                {
                    GameManager.control.Save();
                    SceneManager.LoadSceneAsync("Alps_Main");
                }
                else if (lastCabin == "EditorCabin")
                {
                    StartCoroutine(Save());
                    if (!CustomLevelManager.control.LoadLevel_Play)
                        return;
                }
                GameObject.Find("QuitConfirmDialog").SetActive(false);
            }
        }

        public IEnumerator Save()
        {
            if (!CustomLevelManager.control.inFullPlaytestMode && !CustomLevelManager.control.LoadLevel_Play)
            {
                levelEditorManager.Save(false, false);
                while (!levelEditorManager.hasSavedLevelRecently)
                {
                    yield return null;
                }
                GameObject.Find("SaveConfirmDialog").SetActive(false);
            }


            SceneManager.LoadSceneAsync("Peaks_CustomLevel_Staging"); 
        }
    }
}
