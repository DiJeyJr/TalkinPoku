using UnityEngine;

// ============================================================================
// PetAccessories.cs
// QUE HACE: dibuja sobre Poku la ropa que tiene puesta (gorra, anteojos,
//           bufanda...). Crea un SpriteRenderer hijo por ranura y lo ubica en el
//           punto de anclaje de la evolucion actual.
// DONDE ESTA: en el objeto "Poku" de SampleScene.
// PARA CAMBIAR: la posicion/tamano de UN accesorio -> su asset (Item_Ropa_*):
//               "Escala Mundo", "Desplazamiento" y "Ajustes" por etapa.
//               Donde va cada ranura en cada evolucion -> los assets
//               Data/Pet/Apariencia_* (anclas y "Escala Accesorios").
// FORMULA: posicion = ancla de la ranura + desplazamiento del item * escala de
//          la etapa + ajuste de la etapa.
// ============================================================================

// Va en la mascota. Dibuja la ropa que tiene puesta, una por ranura, en el ancla que dice la
// apariencia de la etapa: asi el sombrero sigue en la cabeza cuando Poku evoluciona.
[RequireComponent(typeof(SpriteRenderer))]
public class PetAccessories : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    [SerializeField] private PetMoodController cara;
    // Dormido esta hecho una bolita: la ropa quedaria flotando, asi que se esconde.
    [SerializeField] private NeedsSystem necesidades;

    // El sprite del cuerpo de Poku.
    private SpriteRenderer cuerpo;
    // Un SpriteRenderer por ranura (Cabeza, Cara, Cuello, Espalda).
    private SpriteRenderer[] puestos;

    // Crea los hijos "Accesorio_Cabeza", "Accesorio_Cara", etc.
    private void Awake()
    {
        cuerpo = GetComponent<SpriteRenderer>();
        int ranuras = System.Enum.GetValues(typeof(AccessorySlot)).Length;
        puestos = new SpriteRenderer[ranuras];
        for (int i = 0; i < ranuras; i++)
        {
            GameObject hijo = new GameObject("Accesorio_" + (AccessorySlot)i);
            hijo.transform.SetParent(transform, false);
            puestos[i] = hijo.AddComponent<SpriteRenderer>();
            puestos[i].sortingLayerID = cuerpo.sortingLayerID;
        }
    }

    // Se redibuja cuando cambia el inventario, la evolucion o si se duerme.
    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        cara.AparienciaCambiada += Refrescar;
        necesidades.DormidoCambiado += AlDormir;
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
        cara.AparienciaCambiada -= Refrescar;
        necesidades.DormidoCambiado -= AlDormir;
    }

    private void Start()
    {
        Refrescar();
    }

    private void AlDormir(bool dormido) => Refrescar();

    // Para cada ranura: si tiene algo puesto, pone su sprite, su orden de dibujo, posicion y escala.
    private void Refrescar()
    {
        PetAppearance apariencia = cara.Apariencia;
        Sprite referencia = apariencia != null ? apariencia.Normal : null;

        for (int i = 0; i < puestos.Length; i++)
        {
            ItemDefinition item = inventario.PuestoEn((AccessorySlot)i);
            bool mostrar = item != null && item.SpriteMundo != null && referencia != null && !necesidades.Dormido;
            puestos[i].enabled = mostrar;
            if (!mostrar) continue;

            puestos[i].sprite = item.SpriteMundo;
            // Capa negativa (las alas) va detras del cuerpo; el resto, adelante.
            puestos[i].sortingOrder = cuerpo.sortingOrder + (item.Capa < 0 ? item.Capa : 1 + item.Capa);
            ItemDefinition.AjusteEtapa ajuste = item.AjusteDe(apariencia);
            float escalaAjuste = ajuste.escala > 0f ? ajuste.escala : 1f;
            Vector2 lugar = (Vector2)PuntoDelSprite(referencia, apariencia.Ancla(item.Ranura))
                + item.Desplazamiento * apariencia.EscalaAccesorios + ajuste.desplazamiento;
            puestos[i].transform.localPosition = lugar;
            puestos[i].transform.localScale = Vector3.one * (apariencia.EscalaAccesorios * item.EscalaMundo * escalaAjuste);
        }
    }

    // Pasa una fraccion del dibujo a coordenadas locales, teniendo en cuenta el pivot del sprite.
    private static Vector3 PuntoDelSprite(Sprite sprite, Vector2 fraccion)
    {
        Vector2 pixeles = Vector2.Scale(fraccion, sprite.rect.size) - sprite.pivot;
        return pixeles / sprite.pixelsPerUnit;
    }
}
