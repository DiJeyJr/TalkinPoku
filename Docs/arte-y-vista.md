# Vista del juego y especificación de arte

Documento de referencia para producir los assets. Complementa al
[GDD](../Grupo12_GDD.md), no lo reemplaza: acá están los números concretos que el GDD no fija.

## La vista

**Frontal plana, una habitación por pantalla, en vertical.** La mascota va grande, centrada,
en la franja media. Para cambiar de habitación se hace swipe horizontal.

Es el encuadre clásico del género (Pou, My Talking Tom, Tamagotchi). Se eligió sobre las
alternativas (casa en corte con varias habitaciones a la vista, o isométrica) porque:

- El GDD pide que la mascota sea el principal foco visual y que sus animaciones dejen leer su
  estado. Con la cámara alejada eso deja de leerse.
- La ropa por capas sólo es barata de dibujar de frente. En isométrica cada prenda necesitaría
  varios ángulos.
- En isométrica cada mueble hay que dibujarlo en perspectiva, y el GDD pide muchas decoraciones.

## Números de pantalla

El juego corre en **portrait fijo**. Resolución de referencia **1080 × 1920**, a **100 píxeles por
unidad** (PPU). O sea: 1080 px = 10.8 unidades de mundo, 1920 px = 19.2 unidades.

La cámara es ortográfica y está configurada para **garantizar el ancho**: siempre se ven las 10.8
unidades completas, sin importar el celular. Lo que cambia entre dispositivos es **cuánto se ve a lo
alto**:

| Pantalla | Proporción | Alto visible |
|---|---|---|
| 16:9 (1080 × 1920) | 0.5625 | 1920 px |
| 19.5:9 (1080 × 2340) | 0.4615 | 2340 px |
| 20:9 (1080 × 2400) | 0.45 | 2400 px |

En un celular alto se ve **más** pared arriba y más piso abajo. Nunca se recorta a los costados.

### Consecuencia directa para el arte

**Los fondos de habitación se entregan en 1080 × 2400 px.**

Todo lo importante —muebles, objetos interactuables, el cuadro, la mascota— va dentro de los
**1920 px centrales**. Los 240 px de arriba y los 240 de abajo son relleno que sólo aparece en
celulares altos: pared lisa arriba, piso liso abajo. Si ahí ponen algo importante, en un iPhone
viejo no se ve.

## Zonas de la pantalla

Sobre los 1920 px de referencia, de arriba hacia abajo:

- **0 – 190 px (10%): HUD.** Barras de hambre, higiene, diversión, energía y felicidad; monedas,
  gemas y nivel. Zona reservada, no poner nada del mundo acá.
- **190 – 1530 px: la habitación y la mascota.** Es donde se juega.
- **1530 – 1920 px (20%): botones de acción.** Los controles frecuentes van abajo, que es donde
  llegan los pulgares.

Los porcentajes salen de la clase 4 de la materia y son guías, no leyes. Lo que sí es fijo: todo lo
que se toca tiene que medir al menos **48 dp** (unos 120–150 px a esta resolución de referencia), y
el área táctil puede ser más grande que el dibujo.

Además el HUD y los botones se recortan al **Safe Area** del dispositivo, para no quedar tapados por
el notch ni por la barra de gestos. El fondo de la habitación sí puede pasar por detrás del notch.

## La mascota

- Alto aproximado: **600 px** (6 unidades), o sea cerca de un tercio de la pantalla de referencia.
  Es lo que hace que sea el foco visual.
- Se para con los pies apoyados alrededor de los **500 px desde el borde inferior** de los 1920 de
  referencia, para quedar por encima de la banda de botones.
- **Pivot en Bottom** (0.5, 0). Así apoya bien en el piso aunque cambie de tamaño entre evoluciones.

### Ropa y accesorios

Todas las prendas se dibujan **sobre el mismo lienzo que el cuerpo, alineadas y con el mismo
pivot**. Si una remera se entrega recortada a su propio tamaño, no va a calzar.

Orden de capas dentro de la mascota (Order in Layer):

```
 0   cuerpo base
 5   expresión (ojos y boca según el estado)
10   piernas / pantalón
20   torso / remera
30   cabeza / sombrero
40   accesorio
```

La mascota lleva un `SortingGroup`, así que todas sus capas se ordenan como un bloque frente al
resto de la escena. Sin eso, un sombrero puede terminar detrás de un mueble.

### Expresiones

El GDD pide que se lea el estado. Mínimo cinco: **normal, hambre, sueño, sucio, feliz**. Se dibujan
sólo los ojos y la boca, sobre transparente, en el mismo lienzo que el cuerpo, y se intercambian por
código.

## Habitaciones

Cinco, cada una con su fondo de 1080 × 2400:

| Habitación | Para qué | Qué necesita además del fondo |
|---|---|---|
| Sala principal | Interactuar con la mascota | El cuadro dibujable colgado en la pared |
| Cocina | Alimentar | Superficie donde aparece la comida |
| Baño | Limpiar | Bañera o ducha |
| Dormitorio | Descansar y cambiar la ropa | Cama, placard |
| Sala de juegos | Entrar a los minijuegos | Accesos visuales a cada minijuego |

Cada habitación define **puntos de anclaje** para decoraciones: posiciones fijas donde el jugador
puede colocar muebles. Se marcan en la escena; el arte sólo necesita respetar un tamaño máximo por
anclaje para que nada se superponga.

## Decoraciones y props

- PNG con alfa, mismo PPU (100).
- **Pivot en Bottom** para lo que se apoya en el piso, **Center** para lo que cuelga de la pared.
- Se ordenan por `Order in Layer` según su distancia visual, no por posición en Z.

## Formato de entrega

- **PNG-24 con canal alfa**, sin alfa premultiplicado, espacio sRGB.
- Import en Unity: Sprite (2D and UI), **Pixels Per Unit = 100**, Mesh Type Full Rect,
  **Generate Mip Maps desactivado**, compresión ASTC.
- No hace falta entregar potencias de dos: los sprites van a un Sprite Atlas.
- Un atlas por sistema: uno de UI, uno de la mascota, uno por habitación, uno por minijuego.
  No mezclar cosas que nunca se muestran juntas, porque carga memoria de más.

### Nombres

```
Room_<Habitacion>_BG          Room_Cocina_BG
Pet_<Etapa>_<Parte>           Pet_Poku_Body, Pet_Poku_Face_Hambre
Cloth_<Slot>_<Nombre>         Cloth_Torso_RemeraRoja, Cloth_Cabeza_GorroLana
Deco_<Categoria>_<Nombre>     Deco_Mueble_MesaRedonda
UI_<Elemento>                 UI_Icono_Hambre, UI_Moneda
```

### Dónde van

```
Assets/Art/
  Rooms/        fondos de habitación
  Pet/          cuerpo, expresiones, evoluciones
  Clothes/      prendas por slot
  Deco/         muebles y decoraciones
  UI/           iconos, botones, barras
  Minigames/<nombre>/
```

## Sorting layers

Orden de atrás hacia adelante:

```
Fondo → Decoracion → Mascota → Objetos → Efectos → UIMundo
```

## Primer entregable de arte

Lo mínimo para tener algo jugable y poder probarlo en un celular. No hace falta que esté terminado,
alcanza con que esté a escala:

1. Fondo de la **Sala principal** (1080 × 2400).
2. Mascota **Poku**: cuerpo base + las cinco expresiones.
3. Iconos de las cinco necesidades, más moneda y gema.
4. Un botón genérico, para las acciones.

Con eso se arma la primera escena y se valida que las proporciones funcionen en un teléfono real,
que es lo único que dice la verdad sobre tamaños táctiles.
