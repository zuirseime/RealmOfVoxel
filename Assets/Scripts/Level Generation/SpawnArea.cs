using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private Vector3 _center;
    [SerializeField] private Vector3 _size;

    [SerializeField] private List<GameObject> _entityPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        //Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 position = Vector3.zero;
            var entity = _entityPrefabs.OrderBy(e => Random.value).First();

            position = transform.position + _center + new Vector3(
                Random.Range(-_size.x, _size.x),
                Random.Range(-_size.y, _size.y),
                Random.Range(-_size.z, _size.z)
            ) / 2f;
            position.y = 0;

            Instantiate(entity, position, Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position + _center, _size);
    }
}
