using NaughtyAttributes;
using UnityEngine;

public class EntityDataHolder : MonoBehaviour
{
    [field: SerializeField, Expandable] public EntityData Data { get; private set; }
}