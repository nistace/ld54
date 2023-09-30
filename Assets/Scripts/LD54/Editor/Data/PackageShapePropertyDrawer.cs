using LD54.Data;
using UnityEditor;
using UnityEngine;

namespace LD54Editor.Data {
	[CustomPropertyDrawer(typeof(PackageShape))]
	public class PackageShapePropertyDrawer : PropertyDrawer {
		private int width;
		private int length;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			length = property.FindPropertyRelative("lines").arraySize;
			width = length == 0 ? 0 : property.FindPropertyRelative("lines").GetArrayElementAtIndex(0).FindPropertyRelative("cells").arraySize;
			return (1 + length) * EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var newWidth = EditorGUI.IntField(new Rect(position.x, position.y, position.width * .4f, EditorGUIUtility.singleLineHeight), width);
			EditorGUI.LabelField(new Rect(position.x + position.width * .4f, position.y, position.width * .2f, EditorGUIUtility.singleLineHeight), "x",
				new GUIStyle { alignment = TextAnchor.MiddleCenter });
			var newLength = EditorGUI.IntField(new Rect(position.x + position.width * .6f, position.y, position.width * .4f, EditorGUIUtility.singleLineHeight), length);
			newWidth = Mathf.Clamp(newWidth, 1, 5);
			newLength = Mathf.Clamp(newLength, 1, 5);
			if (width != newWidth || length != newLength) {
				ResizeShape(property, newWidth, newLength);
			}

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			for (var x = 0; x < width; ++x)
			for (var y = 0; y < length; ++y) {
				var cellPropertyPosition = new Rect(position.x + EditorGUIUtility.singleLineHeight * x, position.y + EditorGUIUtility.singleLineHeight * (y + 1), EditorGUIUtility.singleLineHeight,
					EditorGUIUtility.singleLineHeight);
				var cellProperty = property.FindPropertyRelative("lines").GetArrayElementAtIndex(y).FindPropertyRelative("cells").GetArrayElementAtIndex(x);
				EditorGUI.PropertyField(cellPropertyPosition, cellProperty, GUIContent.none);
			}

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		private void ResizeShape(SerializedProperty property, int newWidth, int newLength) {
			width = newWidth;
			length = newLength;

			var linesProperty = property.FindPropertyRelative("lines");
			while (linesProperty.arraySize < length) {
				linesProperty.InsertArrayElementAtIndex(linesProperty.arraySize);
			}
			while (linesProperty.arraySize > length) {
				linesProperty.DeleteArrayElementAtIndex(linesProperty.arraySize - 1);
			}

			for (var i = 0; i < length; ++i) {
				var cellsProperty = linesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("cells");
				while (cellsProperty.arraySize < width) {
					cellsProperty.InsertArrayElementAtIndex(cellsProperty.arraySize);
				}
				while (cellsProperty.arraySize > width) {
					cellsProperty.DeleteArrayElementAtIndex(cellsProperty.arraySize - 1);
				}
			}
		}
	}
}