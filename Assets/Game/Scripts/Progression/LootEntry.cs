using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Una entrada en la tabla de loot de un enemigo.
/// Define el objeto que puede soltarse y la probabilidad de que ocurra.
/// </summary>
[Serializable]
public class LootEntry
{
    [Tooltip("Objeto que puede soltarse.")]
    public ItemData item;

    [Tooltip("Probabilidad de soltar este objeto (0 = nunca, 1 = siempre).")]
    [Range(0f, 1f)]
    public float dropChance = 0.25f;

    [Tooltip("Cantidad mínima que se suelta al activarse.")]
    [Min(1)]
    public int minQuantity = 1;

    [Tooltip("Cantidad máxima que se suelta al activarse.")]
    [Min(1)]
    public int maxQuantity = 1;

    [Tooltip("Dinero que también se suelta con este objeto (puede ser 0).")]
    [Min(0)]
    public int moneyDrop = 0;
}
