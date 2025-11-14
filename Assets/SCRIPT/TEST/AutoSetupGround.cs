using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Script này chỉ có ý nghĩa sử dụng trong Unity Editor để setup scene
public class AutoSetupGround : MonoBehaviour
{
    // Hàm Reset() được gọi khi component được thêm vào hoặc khi nhấn nút Reset trong Inspector
    void Reset()
    {
        // --- 1. Thêm BoxCollider2D nếu chưa có ---
        if (GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("BoxCollider2D đã được thêm vào: " + gameObject.name);
        }

        // --- 2. Xử lý Tag "Ground" ---
#if UNITY_EDITOR
        // Tự động thêm Tag "Ground" nếu chưa tồn tại
        EnsureTagExists("Ground");
#endif
        gameObject.tag = "Ground";

        // --- 3. Xử lý Layer "Ground" ---
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
#if UNITY_EDITOR
            // Tự động thêm Layer "Ground" nếu chưa tồn tại
            groundLayer = EnsureLayerExists("Ground");
#endif
        }
        
        // Gán Layer cho GameObject
        if (groundLayer != -1)
        {
            gameObject.layer = groundLayer;
        }

        Debug.Log("✅ Auto setup Ground thành công cho " + gameObject.name + " (Layer: Ground, Tag: Ground)");
    }

#if UNITY_EDITOR
    // Hàm hỗ trợ đảm bảo một Tag tồn tại
    private void EnsureTagExists(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
        );
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagName)) { found = true; break; }
        }

        if (!found)
        {
            // Thêm Tag vào vị trí đầu tiên còn trống
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
            tagManager.ApplyModifiedProperties();
            Debug.Log($"Tag '{tagName}' đã được tạo.");
        }
    }

    // Hàm hỗ trợ đảm bảo một Layer tồn tại và trả về index của Layer đó
    private int EnsureLayerExists(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
        );
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        
        for (int i = 8; i < layersProp.arraySize; i++) // Layer tùy chỉnh bắt đầu từ index 8
        {
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"Layer '{layerName}' đã được tạo.");
                return i;
            }
        }
        return -1; // Không tìm thấy chỗ trống để tạo Layer mới
    }
#endif
}