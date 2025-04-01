using System;
using UnityEngine;

public class CharmEquipper : MonoBehaviour
{
    public event EventHandler<Charm> CharmChanged;

    [SerializeField] private Transform _container;
    [SerializeField] private Charm _charm;

    public Charm Charm => _charm;

    public void Equip(Charm newCharm)
    {
        Player player = GetComponent<Player>();

        if (_charm != null)
        {
            Instantiate(_charm, transform.position, Quaternion.identity);
            _charm.RemoveEffect(player);
            Destroy(_charm.gameObject);
        }

        _charm = Instantiate(newCharm, _container);
        _charm.ApplyEffect(player);

        OnCharmChanged();
    }

    private void OnCharmChanged()
    {
        CharmChanged?.Invoke(this, _charm);
    }
}
