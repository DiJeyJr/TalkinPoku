using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ============================================================================
// RoomDecor.cs
// QUE HACE: dibuja en la Sala principal los muebles que el jugador coloco. Cada
//           mueble va en su lugar fijo (no se arrastran).
// DONDE ESTA: en "Habitaciones/Sala principal" de SampleScene.
// PARA CAMBIAR: donde van los dos cuadros -> lugaresCuadro (Inspector).
//               donde va la lampara de mesa si no hay mesita -> lamparaSinMesita.
//               donde va cualquier otro mueble -> "Posicion Inicial" de su asset.
//               tamano de un mueble -> "Escala Mundo" de su asset.
// ============================================================================

// Va en la sala. Dibuja los muebles que el jugador coloco, cada uno en su lugar fijo: no se arrastran,
// asi la sala siempre queda armada y nada tapa a Poku ni a los botones.
public class RoomDecor : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    // El fondo de la sala: los muebles van en su capa de dibujo, encima de el.
    [SerializeField] private SpriteRenderer fondo;

    [Header("Lugares")]
    // [AJUSTE] Posiciones de los cuadros en la pared (izquierda y derecha).
    // Los dos lugares de cuadro en la pared: el primero que se cuelga va al primero.
    [SerializeField] private Vector2[] lugaresCuadro = { new Vector2(-3.45f, 4.15f), new Vector2(3.45f, 4.15f) };
    // [AJUSTE] Posicion de la lampara de mesa cuando no hay mesita.
    // Sin la mesita abajo, la lampara de mesa va al piso en el mismo lugar.
    [SerializeField] private Vector2 lamparaSinMesita = new Vector2(3.2f, -0.5f);

    // Los objetos de mueble creados en la escena.
    private readonly List<GameObject> muebles = new List<GameObject>();
    // Los ids con los que se armo la sala la ultima vez.
    private readonly List<string> armados = new List<string>();

    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        Refrescar();
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
    }

    // Borra los muebles dibujados y los vuelve a crear segun lo que esta colocado.
    private void Refrescar()
    {
        // El inventario avisa por cualquier cambio (comer, comprar ropa): solo se rearma si cambio lo
        // que hay colocado.
        if (armados.SequenceEqual(inventario.Colocados)) return;
        armados.Clear();
        armados.AddRange(inventario.Colocados);

        // Son pocos muebles: rearmarlos todos es mas simple que seguir cual cambio, y un cuadro que se
        // saca puede correr al otro de lugar.
        foreach (GameObject mueble in muebles) Destroy(mueble);
        muebles.Clear();

        // Primero mira si hay mesita (cambia donde va la lampara de mesa).
        bool hayMesita = false;
        foreach (ItemDefinition item in ColocadosEnOrden())
        {
            if (item.Lugar == FurnitureSpot.Mesita) hayMesita = true;
        }

        // Despues crea cada mueble en su posicion.
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

    // Recorre los ids colocados y devuelve sus assets (solo muebles).
    private IEnumerable<ItemDefinition> ColocadosEnOrden()
    {
        foreach (string id in inventario.Colocados)
        {
            ItemDefinition item = inventario.Catalogo.Buscar(id);
            if (item != null && item.Categoria == ItemCategory.Decoracion) yield return item;
        }
    }

    // Crea el GameObject del mueble con su sprite, escala, espejado y orden de dibujo.
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
