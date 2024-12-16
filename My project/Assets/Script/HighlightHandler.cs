using UnityEngine;

public class HighlightHandler : MonoBehaviour
{
    private GameObject _currentTarget;
    private Camera _camera;

    private Color _originalColor;
    private Color highlightColor = Color.white;

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

    public void ApplyHighlight(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            _originalColor = renderer.material.color;
            renderer.material.color = highlightColor * 3;
        }
    }

    public void ClearHighlight()
    {
        if (_currentTarget != null)
        {
            Renderer renderer = _currentTarget.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = _originalColor;
            }
            _currentTarget = null;
        }
    }

    public GameObject GetCurrentTarget()
    {
        return _currentTarget;
    }
}
