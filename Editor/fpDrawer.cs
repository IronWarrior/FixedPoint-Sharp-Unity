using UnityEngine;
using UnityEditor;

namespace Deterministic.FixedPoint
{
    [CustomPropertyDrawer(typeof(fp))]
    public class fpDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            fp value = property.GetValue<fp>();

            EditorGUI.BeginProperty(position, label, property);
            Rect r = EditorGUI.PrefixLabel(position, label);
            string inputText = EditorGUI.TextField(r, value.AsDouble.ToString("0.#####"));
            EditorGUI.EndProperty();

            if (GUI.changed)
            {
                Undo.RecordObjects(property.serializedObject.targetObjects, "Undo Inspector");

                if (float.TryParse(inputText, out float f))
                    property.SetValue(fp.ParseUnsafe(f));
                else
                    property.SetValue<fp>(0);
            }
        }
    }

    [CustomPropertyDrawer(typeof(fp2))]
    public class fp2Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height -= DrawMultiplePropertyFields.BottomSpacing;

            label = EditorGUI.BeginProperty(position, label, property);

            var contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var labels = new[] { new GUIContent("X"), new GUIContent("Y") };
            var properties = new[] { property.FindPropertyRelative("x"), property.FindPropertyRelative("y") };

            DrawMultiplePropertyFields.Draw(contentRect, labels, properties, 3);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + DrawMultiplePropertyFields.BottomSpacing;
        }
    }

    [CustomPropertyDrawer(typeof(fp3))]
    public class fp3Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height -= DrawMultiplePropertyFields.BottomSpacing;

            label = EditorGUI.BeginProperty(position, label, property);

            var contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var labels = new[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
            var properties = new[] { property.FindPropertyRelative("x"), property.FindPropertyRelative("y"), property.FindPropertyRelative("z") };

            DrawMultiplePropertyFields.Draw(contentRect, labels, properties, 3);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + DrawMultiplePropertyFields.BottomSpacing;
        }
    }

    class DrawMultiplePropertyFields
    {
        public const float SubLabelSpacing = 4;
        public const float BottomSpacing = 2;

        // Sourced from dazeili: https://forum.unity.com/threads/making-a-proper-drawer-similar-to-vector3-how.385532/#post-5980577
        public static void Draw(Rect pos, GUIContent[] subLabels, SerializedProperty[] props, int spaces)
        {
            // Backup GUI settings.
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;

            // Draw properties.
            var propsCount = props.Length;
            var width = (pos.width - (spaces - 1) * SubLabelSpacing) / spaces;
            var contentPos = new Rect(pos.x, pos.y, width, pos.height);

            EditorGUI.indentLevel = 0;

            for (var i = 0; i < propsCount; i++)
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x;
                EditorGUI.PropertyField(contentPos, props[i], subLabels[i]);
                contentPos.x += width + SubLabelSpacing;
            }

            // Restore GUI settings.
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indent;
        }
    }
}