using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP
{
    [CustomEditor(typeof(GAgentVisual))]
    [CanEditMultipleObjects]
    public class GAgentVisualEditor : Editor
    {


        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            GAgentVisual agent = (GAgentVisual) target;
            GUILayout.Label("Name: " + agent.name);
            GUILayout.Label("Current Action: " + agent.gameObject.GetComponent<GAgent>().CurrentAction);
            GUILayout.Label("Actions: ");
            foreach (GAction a in agent.gameObject.GetComponent<GAgent>().Actions)
            {
                string pre = "";
                string eff = "";

                foreach (KeyValuePair<string, int> p in a.DicPreConditions)
                    pre += p.Key + ", ";
                foreach (KeyValuePair<string, int> e in a.DicAfterEffects)
                    eff += e.Key + ", ";

                GUILayout.Label("====  " + a.ActionName + "(" + pre + ")(" + eff + ")");
            }

            GUILayout.Label("Goals: ");
            foreach (KeyValuePair<SubGoal, int> g in agent.gameObject.GetComponent<GAgent>().Goals)
            {
                GUILayout.Label("---: ");
                foreach (KeyValuePair<string, int> sg in g.Key.DicSubGoal)
                    GUILayout.Label("=====  " + sg.Key);
            }

            GUILayout.Label("Beliefs: ");
            foreach (KeyValuePair<string, int> sg in agent.gameObject.GetComponent<GAgent>().Beliefs.GetStates())
            {
                GUILayout.Label("=====  " + sg.Key);
            }

            GUILayout.Label("Inventory: ");
            foreach (GameObject g in agent.gameObject.GetComponent<GAgent>().Inventory.items)
            {
                GUILayout.Label("====  " + g.tag);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}