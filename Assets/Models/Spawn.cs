using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    public GameObject unitPrefab;
    public GameObject previewPrefab;
    public LayerMask groundLayer;

    public float maxSampleDistance = 0.1f;

    GameObject previewInstance;
    bool isPlacing = false;


    private void Start()
    {
        
    }

    void Update()
    {
        // Press 1 to start placing (temp test)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartPlacement();
        }

        if (!isPlacing) return;

        UpdatePreview();

        // Left click = place
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawn();
        }

        // Right click = cancel
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    void StartPlacement()
    {
        isPlacing = true;

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = Instantiate(previewPrefab);
    }

    void CancelPlacement()
    {
        isPlacing = false;

        if (previewInstance != null)
            Destroy(previewInstance);
    }

    void UpdatePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 point = hit.point;

            NavMeshHit navHit;

            bool isValid = false;

            if (NavMesh.SamplePosition(point, out navHit, maxSampleDistance, NavMesh.AllAreas))
            {
                float dist = Vector3.Distance(point, navHit.position);

                if (dist <= maxSampleDistance)
                {
                    isValid = true;
                    previewInstance.transform.position = navHit.position;
                }
            }

            // If invalid, still follow mouse (optional)
            if (!isValid)
                previewInstance.transform.position = point;

            SetPreviewColor(isValid);
        }
    }

    void TrySpawn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 point = hit.point;

            NavMeshHit navHit;

            if (NavMesh.SamplePosition(point, out navHit, maxSampleDistance, NavMesh.AllAreas))
            {
                float dist = Vector3.Distance(point, navHit.position);

                if (dist <= maxSampleDistance)
                {
                    Instantiate(unitPrefab, navHit.position, Quaternion.identity);
                }
            }
        }
    }

    void SetPreviewColor(bool valid)
    {
        var renderer = previewInstance.GetComponentInChildren<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = valid ? Color.green : Color.red;
        }
    }
}