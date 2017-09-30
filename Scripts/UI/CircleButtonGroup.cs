﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MRUI
{
    [ExecuteInEditMode]
    public class CircleButtonGroup : MonoBehaviour
    {
        // TODO: combine with MRUiButtonGroup?
        [Tooltip("amount of buttons that can be selected at once. For a select-like behaviour use 1, 0 means all options can be selected")]
        public int maxSelect = 0;

        public GameObject CircleButtonPrefab;

        public List<ButtonData> data;

        // "number of segments the whole circle consists of. this defines the amount of vertices/triangles used"
        // TODO: calculate - espeically for 5 and 7
        private int parts = 36;

        [Tooltip("radius from center to circle")]
        public float innerRadius = .04f;

        [Tooltip("radius from center to outer circle")]
        public float outerRadius = .1f;

        [Tooltip("width of the circle")]
        public float width = .04f;

        [SerializeField]
        public OnSelectedEvent OnSelected;

        /// <summary>
        /// user selected one of the options by pressing a button 
        /// </summary>
        public void OnButtonPressed(ButtonData data)
        {
            if (OnSelected != null)
            {
                OnSelected.Invoke(data.data);
            }
        }

        void updateButtons()
        {
            if (this == null)
            {
                // because this is a delayed call it might be that the object has already been destroyed
                return;
            }
            destroyButtons();

            switch (data.Count)
            {
                case 10:
                case 8:
                case 5:
                    parts = 40;
                    break;
                case 7:
                    parts = 49;
                    break;
                default:
                    parts = 36;
                    break;
            }

            if (data != null && CircleButtonPrefab != null && data.Count > 0)
            {
                // create new buttons and rotate them
                float angle = 360 / data.Count;
                for (int i = 0; i < data.Count; i++)
                {
                    ButtonData buttonData = data[i];
                    GameObject btnInst = Instantiate(CircleButtonPrefab, transform);

                    MRUI.Button btn = btnInst.GetComponent<MRUI.Button>();
                    btn.OnPressed.AddListener(delegate { OnButtonPressed(buttonData); });
                    btn.data = buttonData;
                    btn.UpdateData();
                    
                    
                    CircleButtonSegment segment = btnInst.GetComponentInChildren<CircleButtonSegment>();
                    segment.parts = parts;
                    segment.innerRadius = innerRadius;
                    segment.outerRadius = outerRadius;
                    segment.width = width;
                    segment.segments = data.Count;
                    int add = 0;
                    switch (data.Count)
                    {
                        case 5:
                            add = -16;
                            break;
                        case 4:
                            add = -45;
                            break;
                        case 3:
                            add = -30;
                            break;
                    }
                    segment.setAngle(angle*i+add);

                }
            }
        }

        void destroyButtons()
        {
            List<GameObject> children = new List<GameObject>();
            // we can not delete items from a list while iterating over it
            foreach (Transform child in transform)
            {
                children.Add(child.gameObject);
            }
            for (int i = children.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(children[i]);
            }
        }

        public void UpdateData()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                updateButtons();
            };
#else
            updateButtons();
#endif
        }
    }
}
