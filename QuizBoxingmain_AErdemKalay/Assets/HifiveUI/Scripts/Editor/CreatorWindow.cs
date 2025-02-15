﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace HifiveUI.Scripts.Editor
{
    public class CreatorWindow : EditorWindow
    {
        private List<UiDrawer> _contents = new List<UiDrawer>() { };
        private List<bool> _selections = new List<bool>();

        private List<string> _contentNames = new List<string>()
        {
            "Home UI",
            "Game UI",
            "Progress Slide",
            "Progress Stage",
            "Level Failed UI",
            "Level Completed UI",
            "Chest Fill",
            "Popup Bonus",
            "Unlock UI",
            "Chest UI",
            "Shop UI"
        };

        private List<string> _contentPaths = new List<string>()
        {
            "Previews/home_prev",
            "Previews/game_prev",
            "Previews/progress_slide_prev",
            "Previews/progress_stage_prev",
            "Previews/level_failed_prev",
            "Previews/level_completed_prev",
            "Previews/chest_fill_prev",
            "Previews/popup_bonus_prev",
            "Previews/unlock_prev",
            "Previews/chest_prev",
            "Previews/shop_prev"
        };

        private List<string> _contentComponents = new List<string>()
        {
            "HomeUI",
            "GameUI",
            "ProgressSlide",
            "ProgressStage",
            "LevelFailedUI",
            "LevelCompletedUI",
            "ChestFill",
            "PopupBonus",
            "UnlockUI",
            "ChestUI",
            "ShopUI"
        };

        private List<GameObject> _prefabs = new List<GameObject>();
        private GUIStyle titleStyle;

        private int selectedContentIndex;

        #region Generated Script Data

        private string fileName;

        private string namespacePartOfScript =
            @"using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
";

        private string classDefinitionStart =
            @"
public class UIManager : MonoBehaviour
{
";

        private string singleton =
            @"
    private static UIManager _instance = null;
    public static UIManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        _instance = this;
    }

    ";
        
        private string startFunc =
            @"

    private void Start()
    {
        if (UnityEngine.iOS.Device.generation.ToString().Contains(""iPad""))
            {
            Debug.Log(""iPad device detected! Canvas Scaler Match Size increased to 1f"");
            CanvasScaler[] scalers = GetComponents<CanvasScaler>();
            foreach (CanvasScaler canvasScaler in scalers)
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
        }else
        {
            Debug.Log(""iPad device not detected! Canvas Scaler Match Size set to default"");
        }
    }

    ";

        private string classDefinitionEnd =
            @"
}
        ";

        #endregion

        private void OnEnable()
        {
            titleStyle = new GUIStyle()
            {
                fontSize = 30,
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold,
            };
            titleStyle.normal.textColor = Color.green;

            // Init contents
            InitContents();

            fileName = Application.dataPath + "/Scripts/Managers/UIManager.cs";
        }

        /// <summary>
        /// Initializes selectable contents of HifiveUI
        /// </summary>
        private void InitContents()
        {
            for (int i = 0; i < _contentNames.Count; i++)
            {
                UiDrawer content = new UiDrawer();
                content.Name = _contentNames[i];
                content.Preview = Resources.Load<Texture2D>(_contentPaths[i]);
                _contents.Add(content);

                _selections.Add(false);
            }

            _prefabs = new List<GameObject>()
            {
                Resources.Load<GameObject>("Prefabs/HomeUI"),
                Resources.Load<GameObject>("Prefabs/GameUI"),
                Resources.Load<GameObject>("Prefabs/ProgressBarSliderPanel"),
                Resources.Load<GameObject>("Prefabs/ProgressBarStagePanel"),
                Resources.Load<GameObject>("Prefabs/LevelFailedUI"),
                Resources.Load<GameObject>("Prefabs/LevelCompletedUI"),
                Resources.Load<GameObject>("Prefabs/ChestFillPanel"),
                Resources.Load<GameObject>("Prefabs/PopupBonusPanel"),
                Resources.Load<GameObject>("Prefabs/UnlockUI"),
                Resources.Load<GameObject>("Prefabs/ChestUI"),
                Resources.Load<GameObject>("Prefabs/ShopUI")
            };
        }

        /// <summary>
        /// Draws selectable contents of HifiveUI
        /// </summary>
        /// <param name="index"></param>
        private void DrawContent(int index)
        {
            EditorGUILayout.BeginHorizontal("box");

            if ((index == 2 || index == 3) && _selections[1] == false
            ) // Disable GameUI sub panels if GameUI not selected
            {
                EditorGUI.BeginDisabledGroup(_selections[1] == false);
                _selections[index] = false;
                _selections[index] = EditorGUILayout.ToggleLeft(_contentNames[index], _selections[index]);
                EditorGUI.EndDisabledGroup();
            }
            else if ((index == 6 || index == 7) && _selections[5] == false
            ) // Disable LevelCompletedUI sub panels if LevelCompletedUI not selected
            {
                EditorGUI.BeginDisabledGroup(_selections[5] == false);
                _selections[index] = false;
                _selections[index] = EditorGUILayout.ToggleLeft(_contentNames[index], _selections[index]);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                _selections[index] = EditorGUILayout.ToggleLeft(_contentNames[index], _selections[index]);
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                selectedContentIndex = index;
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Instantiates all selected UI prefabs from Hifive UI Creator window
        /// </summary>
        private void GenerateUI()
        {
            // Check EventSystem in scene if there is not, instantiate an EventSystem
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("Event System");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            GameObject go = new GameObject("Hifive UI");

            // Instantiate UI prefabs
            for (int i = 0; i < _contents.Count; i++)
            {
                if (_selections[i])
                {
                    if ((i == 2 || i == 3) && _selections[1] == true
                    ) // Instantiate GameUI sub panels if GameUI is selected
                    {
                        _prefabs[i] = Instantiate(_prefabs[i]);
                        _prefabs[i].transform.SetParent(_prefabs[1].transform);
                        _prefabs[i].GetComponent<RectTransform>().localScale = Vector3.one;
                        _prefabs[i].GetComponent<RectTransform>().offsetMin = Vector2.zero;
                        _prefabs[i].GetComponent<RectTransform>().offsetMax = Vector2.zero;
                    }
                    else if ((i == 6 || i == 7) && _selections[5] == true
                    ) // Instantiate LevelCompletedUI sub panels if LevelCompletedUI is selected
                    {
                        _prefabs[i] = Instantiate(_prefabs[i]);
                        _prefabs[i].transform.SetParent(_prefabs[5].transform);
                        _prefabs[i].GetComponent<RectTransform>().localScale = Vector3.one;
                        _prefabs[i].GetComponent<RectTransform>().offsetMin = Vector2.zero;
                        _prefabs[i].GetComponent<RectTransform>().offsetMax = Vector2.zero;
                    }
                    else
                    {
                        _prefabs[i] = Instantiate(_prefabs[i]);
                        _prefabs[i].transform.SetParent(go.transform);
                    }

                    _prefabs[i].name = _contents[i].Name;
                }
            }

            // Generate UIManager script
            GenerateUIManager();
        }

        private void GenerateUIManager()
        {
            // Check if file already exists. If yes, delete it.     
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // Create a new file     
            using (FileStream fs = File.Create(fileName))
            {
                // Add Author name, copyright etc
                Byte[] introText =
                    new UTF8Encoding(true).GetBytes(
                        "// This script generated by Hifive UI Creator\n// Hifive Games - Gokhan Yahya TORBA 2020\n\n");
                fs.Write(introText, 0, introText.Length);

                // Add namespaces to text file
                Byte[] namespaceText = new UTF8Encoding(true).GetBytes(namespacePartOfScript);
                fs.Write(namespaceText, 0, namespaceText.Length);

                // Add class definition start part to text file
                Byte[] classDefinitionText = new UTF8Encoding(true).GetBytes(classDefinitionStart);
                fs.Write(classDefinitionText, 0, classDefinitionText.Length);

                // GENERATE CODE HERE!

                GenerateVariables(fs);

                // Add singleton script to text file
                Byte[] singletonText = new UTF8Encoding(true).GetBytes(singleton);
                fs.Write(singletonText, 0, singletonText.Length);
                
                // Add start function to text file
                Byte[] startFuncText = new UTF8Encoding(true).GetBytes(startFunc);
                fs.Write(startFuncText, 0, startFuncText.Length);


                // Add class definition end to text file
                Byte[] classDefinitionEndText = new UTF8Encoding(true).GetBytes(classDefinitionEnd);
                fs.Write(classDefinitionEndText, 0, classDefinitionEndText.Length);
            }

            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath("Assets/Scripts/Managers/UIManager.cs",
                typeof(Object)));
        }

        void GenerateVariables(FileStream fs)
        {
            Byte[] boolText;

            for (int i = 0; i < _contents.Count; i++)
            {
                if (_selections[i])
                {
                    // Variable
                    boolText = new UTF8Encoding(true)
                        .GetBytes("[SerializeField] private " + _contentComponents[i] + " _" + _contentComponents[i] +
                                  ";\n");
                    fs.Write(boolText, 0, boolText.Length);
                }
            }

            boolText = new UTF8Encoding(true)
                .GetBytes("\n");
            fs.Write(boolText, 0, boolText.Length);

            for (int i = 0; i < _contents.Count; i++)
            {
                if (_selections[i])
                {
                    // Property
                    boolText = new UTF8Encoding(true)
                        .GetBytes("public " + _contentComponents[i] + " " + _contentComponents[i].ToUpper() + " => " +
                                  "_" + _contentComponents[i] + ";\n");
                    fs.Write(boolText, 0, boolText.Length);
                }
            }
        }

        /// <summary>
        /// Creates an instance of this editor window and show
        /// </summary>
        [MenuItem("Hifive Games/UI/Creator")]
        public static void ShowCreatorWindow()
        {
            EditorWindow window = GetWindow(typeof(CreatorWindow));
            window.titleContent = new GUIContent("Hifive UI Creator");
            window.minSize = new Vector2(720, 640);
            window.maxSize = new Vector2(720, 640);
            window.Focus();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            ////////////////////////////////////// // Left Panel

            #region Left Panel for selection

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("———————————————————————", titleStyle);
            EditorGUILayout.LabelField("HIFIVE UI CREATOR", titleStyle);
            EditorGUILayout.LabelField("———————————————————————", titleStyle);
            EditorGUILayout.Space(50);

            #endregion

            #region Draw list of selectable UI Contents to left panel

            for (int i = 0; i < 11; i++)
            {
                DrawContent(i);
                EditorGUILayout.Space(3);
            }

            #endregion

            #region Draw Generate UI Button

            EditorGUILayout.Space(50);
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };
            buttonStyle.normal.textColor = Color.green;
            if (GUILayout.Button("Generate UI", buttonStyle, GUILayout.Height(50)))
            {
                Debug.Log("Hifive UI Creator: UI Created!");
                GenerateUI();
            }

            // Draw credits label
            EditorGUILayout.Separator();
            GUILayout.Label("© 2020 Gokhan Yahya TORBA", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Label("HIFIVE GAMES", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();

            #endregion

            ////////////////////////////////////// // Right Panel

            #region Draw right panel for preview

            EditorGUILayout.BeginVertical();
            //EditorGUILayout.LabelField("Preview", EditorStyles.whiteLargeLabel);
            GUILayout.Label(_contents[selectedContentIndex].Preview,
                titleStyle,
                GUILayout.Width(360),
                GUILayout.Height(720));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            #endregion

            //////////////////////////////////////
        }
    }
}

public struct UiDrawer
{
    public string Name { get; set; }
    public Texture2D Preview { get; set; }
}