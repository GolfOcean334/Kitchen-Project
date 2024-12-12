using UnityEngine;

public class HighlightHandler : MonoBehaviour
{
    private GameObject _currentTarget;
    private Camera _camera;

    private Material _originalMaterial;
    public Material highlightMaterial;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        HighlightCollectibles();
    }

    public void HighlightCollectibles()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Collectible"))
            {
                if (_currentTarget != hitObject)
                {
                    ClearHighlight();
                    _currentTarget = hitObject;
                    ApplyHighlight(_currentTarget);
                }
            }
            else
            {
                ClearHighlight();
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    private void ApplyHighlight(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            _originalMaterial = renderer.material;
            renderer.material = highlightMaterial;
        }
    }

    private void ClearHighlight()
    {
        if (_currentTarget != null)
        {
            Renderer renderer = _currentTarget.GetComponent<Renderer>();
            if (renderer != null && _originalMaterial != null)
            {
                renderer.material = _originalMaterial;
            }
            _currentTarget = null;
        }
    }
}
