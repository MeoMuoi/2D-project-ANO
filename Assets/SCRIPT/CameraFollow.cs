using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    // ✅ THAY ĐỔI: Giá trị mặc định thấp hơn (0.2f là chậm)
    [Tooltip("Thời gian để camera đạt được vị trí mục tiêu. Giá trị nhỏ hơn sẽ di chuyển nhanh hơn.")]
    public float smoothSpeed = 0.15f; 
    public Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Map Bounds")]
    public BoxCollider2D mapBounds;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private bool hasBounds = false; 

    void Start()
    {
        cam = GetComponent<Camera>();
        if (mapBounds != null)
        {
            minBounds = mapBounds.bounds.min;
            maxBounds = mapBounds.bounds.max;
            hasBounds = true;
        }
    }

    void LateUpdate()
    {
        if (target == null) 
        {
            Debug.LogWarning("Camera Target (MainCharacter) chưa được gán!");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        
        // ✅ CẬP NHẬT: Thay giá trị cứng 0.2f bằng biến smoothSpeed
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        
        Vector3 finalPosition = smoothedPosition;

        // Xử lý Giới hạn Bản đồ
        if (hasBounds)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;
            
            float clampX = Mathf.Clamp(smoothedPosition.x, minBounds.x + camWidth, maxBounds.x - camWidth);
            float clampY = Mathf.Clamp(smoothedPosition.y, minBounds.y + camHeight, maxBounds.y - camHeight);

            finalPosition = new Vector3(clampX, clampY, smoothedPosition.z);
        }

        transform.position = finalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (mapBounds != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(mapBounds.bounds.center, mapBounds.bounds.size);
        }
    }
}