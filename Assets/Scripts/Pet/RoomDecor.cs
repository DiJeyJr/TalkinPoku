using System.Collections.Generic;
using UnityEngine;

/// Va en la sala. Dibuja los muebles que el jugador coloco, cada uno en su lugar fijo: no se arrastran,
/// asi la sala siempre queda armada y nada tapa a Poku ni a los botones.
public class RoomDecor : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    // El fondo de la sala: los muebles van en su capa de dibujo, encima de el.
    [SerializeField] private SpriteRenderer fondo;

    [Header("Lugares")]
    // Los dos lugares de cuadro en la pared: el primero que se cuelga va al primero.
    [SerializeField] private Vector2[] lugaresCuadro = { new Vector2(-3.45f, 4.15f), new Vector2(3.45f, 4.15f) };
    // Sin la mesita abajo, la lampara de mesa va al piso en el mismo lugar.
    [SerializeField] private Vector2 lamparaSinMesita = new Vector2(3.2f, -0.5f);

    private readonly List<GameObject> muebles = new List<GameObject>();

    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        Refrescar();
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
    }

    private void Refrescar()
    {
        // Son pocos muebles: rearmarlos todos es mas simple que seguir cual cambio, y un cuadro que se
        // saca puede correr al otro de lugar.
        foreach (GameObject mueble in muebles) Destroy(mueble);
        muebles.Clear();

        bool hayMesita = false;
        foreach (ItemDefinition item in ColocadosEnOrden())
        {
            if (item.Lugar == FurnitureSpot.Mesita) hayMesita = true;
        }

        int cuadros = 0;
        foreach (ItemDefinition item in ColocadosEnOrden())
        {
            Vector2 posicion = item.PosicionInicial;
            if (item.Lugar == FurnitureSpot.Cuadro)
            {
                if (cuadros >= lugaresCuadro.Length) continue;
                posicion = lugaresCuadro[cuadros++];
            }
            else if (item.Lugar == FurnitureSpot.LamparaMesa && !hayMesita)
            {
                posicion = lamparaSinMesita;
            }
            muebles.Add(Crear(item, posicion));
        }
    }

    private IEnumerable<ItemDefinition> ColocadosEnOrden()
    {
        foreach (string id in inventario.Colocados)
        {
            ItemDefinition item = inventario.Catalogo.Buscar(id);
            if (item != null && item.Categoria == ItemCategory.Decoracion) yield return item;
        }
    }

    private GameObject Crear(ItemDefinition item, Vector2 posicion)
    {
        GameObject objeto = new GameObject(item.Id);
        objeto.transform.SetParent(transform, false);
        objeto.transform.position = posicion;
        objeto.transform.localScale = Vector3.one * item.EscalaMundo;

        SpriteRenderer dibujo = objeto.AddComponent<SpriteRenderer>();
        dibujo.sprite = item.SpriteMundo;
        dibujo.flipX = item.Espejado;
        dibujo.sortingLayerID = fondo.sortingLayerID;
        dibujo.sortingOrder = fondo.sortingOrder + 1 + item.Capa;
        return objeto;
    }
}
