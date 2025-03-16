using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    private GameObject _selection;

    private Vector3 _selectedPosition;
    private Entity _target;

    public void Select(Vector3 position)
    {
        DiselectTargetIfNeeded();

        _selectedPosition = position;
        _selectedPosition.y = 0;

        if (_prefab != null)
        {
            _selection = Instantiate(_prefab, _selectedPosition, Quaternion.identity);
        }
    }

    public void Select(Entity target)
    {
        Select(target.transform.position);

        GetComponent<Player>().target = target;

        _target = target;
        _target.EntityDied += OnTargetDied;
    }

    private void DiselectTargetIfNeeded()
    {
        DestroySelection();

        if (_target != null)
        {
            GetComponent<Player>().target = null;

            _target.EntityDied -= OnTargetDied;
            _target = null;
        }
    }

    private void Update()
    {
        if (IsNearSelection())
        {
            DestroySelection();
        }

        if (_selection != null && _target != null)
        {
            _selection.transform.position = _target.transform.position;
        }
    }

    private bool IsNearSelection(float distance = 1f)
    {
        return _selection != null && Vector3.Distance(_selection.transform.position, transform.position) <= distance;
    }

    private void DestroySelection()
    {
        if (_selection != null)
        {
            Destroy(_selection);
            _selection = null;

            GetComponent<Player>().ClearDestination();
        }
    }

    private void OnTargetDied(object sender, EventArgs args)
    {
        DiselectTargetIfNeeded();
    }
}
