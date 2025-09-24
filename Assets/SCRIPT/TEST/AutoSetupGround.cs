using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutoSetupGround : MonoBehaviour
{
    void Reset()
    {
        // --- Thêm BoxCollider2D nếu chưa có ---
        if (GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }

        // --- Gán Tag "Ground" ---
        if (!IsTagExists("Ground"))
        {
#if UNITY_EDITOR
            // Nếu chưa có thì tạo mới Tag
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Thêm "Ground" vào list Tag
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals("Ground")) { found = true; break; }
            }
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                tagsProp.GetArrayElementAtIndex(0).stringValue = "Ground";
                tagManager.ApplyModifiedProperties();
            }
#endif
        }
        gameObject.tag = "Ground";

        // --- Gán Layer "Ground" ---
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
#if UNITY_EDITOR
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            for (int i = 8; i < layersProp.arraySize; i++) // layer custom từ 8 trở đi
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(sp.stringValue))
                {
                    sp.stringValue = "Ground";
                    tagManager.ApplyModifiedProperties();
                    break;
                }
            }
#endif
            groundLayer = LayerMask.NameToLayer("Ground");
        }
        gameObject.layer = groundLayer;

        Debug.Log("✅ Auto setup Ground thành công cho " + gameObject.name);
    }

    // Check tag có tồn tại không
    private bool IsTagExists(string tag)
    {
        try { var obj = new GameObject(); obj.tag = tag; DestroyImmediate(obj); return true; }
        catch { return false; }
    }
}
