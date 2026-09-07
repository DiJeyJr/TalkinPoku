# **Game Design Document**

# **“Talkin Poku”**

**![][image1]**

**Materia:** Desarrollo de juegos para móvil

**Integrantes:** Milena Janiot, Martina Dalbene, Renzo Bongiorno

**Grupo:** 12

# **1\. Información general**

**Nombre del juego:** Talkin Poku  
**Género:** Simulación / Mascota virtual / Minijuegos  
**Plataforma:** Dispositivos móviles Android e iOS  
**Modo de juego:** Un jugador  
**Perspectiva:** 2D  
**Público objetivo:** Infantil y juvenil  
**Modelo de negocio:** Free-to-play con compras dentro de la aplicación

---

# **2\. Concepto general**

El juego consiste en cuidar y acompañar a una mascota virtual.

El jugador deberá encargarse de las diferentes necesidades de su mascota, como alimentarla, mantenerla limpia, jugar con ella y personalizar su apariencia. Además, podrá decorar su hogar, comprar diferentes objetos y participar en diversos minijuegos para obtener recompensas.

El objetivo principal es generar un vínculo entre el jugador y su mascota mientras se progresa mediante actividades de cuidado, personalización y entretenimiento.

El juego tendrá una estética 2D colorida, amigable y con colores vibrantes, orientada principalmente a un público infantil.

# **3\. Pilares de diseño** **Cuidado**

El jugador deberá atender las necesidades básicas de la mascota.

Entre ellas se encuentran:

* Alimentación.  
* Higiene.  
* Diversión.  
* Descanso.  
* Bienestar general.

Cada actividad contribuirá a mantener feliz y saludable a la mascota.

### **Personalización**

El jugador podrá personalizar tanto a su mascota como el espacio donde vive.

Podrá adquirir:

* Ropa.  
* Accesorios.  
* Comida.  
* Muebles.  
* Decoraciones.  
* Elementos para diferentes habitaciones.

Esto permitirá que cada jugador pueda crear una mascota y un hogar visualmente diferentes.

### **Minijuegos**

Los minijuegos serán una de las principales formas de obtener monedas. Cada uno tendrá mecánicas diferentes y estará diseñado para ser fácil de comprender, pero progresivamente más desafiante.

### **Progresión**

A medida que el jugador interactúe con la mascota y complete actividades podrá acceder a nuevos objetos, recetas, ropa, decoraciones y contenido.

# **4\. Gameplay principal**

El jugador comenzará con una mascota y una casa básica. Dentro de la casa podrá desplazarse entre distintas habitaciones o sectores, cada uno relacionado con una actividad.

Por ejemplo:

**Cocina:** alimentar a la mascota.

**Baño:** limpiar y asear a la mascota.

**Dormitorio:** descansar y cambiar su ropa.

**Sala de juegos:** acceder a los minijuegos.

**Sala principal:** interactuar con la mascota.

El jugador deberá observar las necesidades de la mascota y realizar acciones para mantener sus indicadores en niveles adecuados.

# **5\. Gameplay Loop**

El ciclo principal de juego será:

1. El jugador ingresa al juego.  
2. Observa el estado de su mascota.  
3. Atiende sus necesidades.  
4. Juega uno o varios minijuegos.  
5. Obtiene monedas como recompensa.  
6. Visita la tienda.  
7. Compra comida, ropa o decoraciones.  
8. Personaliza a su mascota o su hogar.  
9. Desbloquea nuevo contenido.  
10. Continúa cuidando a la mascota.

Este ciclo se repetirá constantemente mientras el jugador obtiene nuevos objetos y posibilidades de personalización.

# **6\. Sistema de necesidades**

La mascota contará con diferentes indicadores que disminuirán progresivamente.

### **Hambre**

Representa cuánto necesita alimentarse la mascota. El jugador deberá darle diferentes alimentos para aumentar esta estadística.

### **Higiene**

Disminuirá con el paso del tiempo y determinadas actividades. Para recuperarla, el jugador deberá llevar a la mascota al baño y limpiarla.

### **Diversión**

Aumentará al interactuar con la mascota o participar en minijuegos.

### **Energía**

La mascota perderá energía progresivamente. Cuando el nivel sea bajo, deberá descansar o dormir.

### **Felicidad**

Funcionará como un indicador general del bienestar de la mascota y estará relacionado con el resto de sus necesidades.

# **7\. Sistema de economía**

El juego contará con dos monedas principales.

## **Monedas**

Serán la moneda común del juego. Se podrán conseguir principalmente mediante:

* Minijuegos.  
* Actividades.  
* Recompensas.  
* Progresión dentro del juego.

Las monedas podrán utilizarse para comprar:

* Alimentos.  
* Ropa.  
* Accesorios.  
* Muebles.  
* Decoraciones.

También existirá la posibilidad de comprar paquetes adicionales de monedas utilizando dinero real.

## **Gemas**

Las gemas serán la moneda premium. Su principal método de obtención será mediante compras con dinero real. Podrán utilizarse para adquirir elementos especiales o exclusivos dentro del juego.

# **8\. Monetización**

El juego tendrá un modelo **Free-to-Play**.

El jugador podrá descargar y jugar gratuitamente, mientras que existirá la posibilidad opcional de realizar compras dentro de la aplicación.

Se podrán comprar:

* Paquetes de monedas.  
* Paquetes de gemas.

Debido a que el público principal del juego es infantil, las compras deberán estar claramente diferenciadas del contenido gratuito y requerir confirmación antes de realizar una transacción.

# **9\. Tienda**

La tienda será el lugar donde el jugador podrá utilizar las monedas y gemas obtenidas. Estará dividida en distintas categorías.

### **Comida**

Permitirá adquirir distintos alimentos para alimentar a la mascota.n Los alimentos podrán tener diferentes valores de recuperación de hambre.

### **Ropa**

Permitirá personalizar la apariencia de la mascota.

Ejemplos:

* Remeras.  
* Pantalones.  
* Vestidos.  
* Sombreros.  
* Disfraces.  
* Accesorios.

### **Decoraciones**

Permitirá cambiar la apariencia de la casa.

Ejemplos:

* Muebles.  
* Lámparas.  
* Alfombras.  
* Cuadros.

# **10\. Minijuegos**

Los minijuegos ofrecerán diferentes experiencias y servirán principalmente para obtener monedas. La dificultad podrá aumentar progresivamente dependiendo de la puntuación del jugador.

## **10.1. Vuelo de obstáculos**

Minijuego inspirado en la mecánica de *Flappy Bird*.

El jugador deberá controlar a la mascota o algún personaje mientras avanza automáticamente por el escenario. Al tocar la pantalla, el personaje realizará un pequeño impulso hacia arriba.  El objetivo será atravesar la mayor cantidad posible de obstáculos sin chocarlos.

**Objetivo:** obtener la mayor puntuación posible.

**Recompensa:** monedas dependiendo de la distancia alcanzada.

---

## **10.2. Cocina**

El jugador podrá combinar diferentes ingredientes para descubrir recetas. Inicialmente tendrá disponibles algunos ingredientes básicos. Al encontrar una combinación correcta, se desbloqueará una nueva receta que quedará registrada en un libro de recetas. Las recetas descubiertas podrán utilizarse posteriormente como alimento para la mascota.

**Objetivo:** descubrir todas las recetas posibles.

**Recompensa:** nuevas recetas, alimentos y monedas.

---

## **10.3. Camino a casa**

Se mostrará durante algunos segundos un camino correcto para llegar hasta la casa de un amigo de la mascota. Luego el camino desaparecerá. El jugador deberá recordar la ruta y reproducirla correctamente. A medida que avance, los caminos serán más largos y complejos.

**Objetivo:** memorizar correctamente cada recorrido.

**Recompensa:** monedas por cada nivel completado.

---

## **10.4. Unir a los personajes**

En el escenario habrá dos pequeños personajes separados por obstáculos.

El jugador deberá dibujar una línea, camino o estructura que permita que ambos personajes puedan encontrarse. Cada nivel presentará una distribución diferente de obstáculos.

**Objetivo:** lograr que los dos personajes se encuentren.

**Recompensa:** monedas por completar cada nivel.

---

## **10.5. Limpieza del medioambiente**

La mascota visitará diferentes escenarios naturales afectados por contaminación.

Algunos escenarios podrán incluir:

* Playa.  
* Bosque.  
* Parque.  
* Río.  
* Ciudad.

El jugador deberá recoger basura y limpiar elementos contaminantes del escenario. A medida que avance, el lugar recuperará progresivamente su aspecto limpio y natural. Además de funcionar como minijuego, esta actividad busca introducir de manera sencilla conceptos relacionados con el cuidado del medioambiente.

**Objetivo:** limpiar completamente cada escenario.

**Recompensa:** monedas y desbloqueo de nuevos escenarios.

---

## **10.6. Atrapar ingredientes**

El jugador controlará un recipiente o personaje ubicado en la parte inferior de la pantalla. Podrá desplazarse horizontalmente de izquierda a derecha. Desde la parte superior caerán distintos objetos. El jugador deberá atrapar únicamente los ingredientes o alimentos correctos. También aparecerán objetos incorrectos. Si el jugador atrapa uno de ellos perderá una vida. El jugador comenzará cada partida con **3 vidas**.

Cuando pierde las tres vidas, finaliza la partida.

**Objetivo:** atrapar la mayor cantidad posible de ingredientes.

**Recompensa:** monedas dependiendo de la puntuación obtenida.

---

# **11\. Sistema de dibujo**

Dentro de la casa habrá un cuadro interactivo. Cuando el jugador toque el cuadro se abrirá una interfaz de dibujo.

El jugador podrá utilizar diferentes herramientas básicas, como:

* Pinceles.  
* Diferentes grosores de línea.  
* Colores.  
* Borrador.  
* Botón para limpiar el dibujo.  
* Botón para guardar.

Una vez guardado, el dibujo realizado aparecerá dentro del cuadro ubicado en la casa de la mascota. Esto permitirá añadir un elemento de creatividad y personalización adicional. El jugador podrá modificar el dibujo cuando quiera.

---

# **12\. Progresión y Evolución**

La mascota contará con un sistema de niveles basado en puntos de experiencia. El jugador obtendrá experiencia principalmente al:

* Alimentar a la mascota.  
* Asearla.  
* Jugar e interactuar con ella.  
* Completar minijuegos.  
* Realizar distintas actividades de cuidado.

A medida que aumente de nivel, la mascota evolucionará y cambiará su apariencia. Cada evolución representará una nueva etapa de crecimiento del personaje.

## **Etapas evolutivas**

La progresión de la mascota estará dividida en diferentes etapas:

| Nivel | Evolución |
| ----- | ----- |
| 1-10 | **Poku** |
| 20-29 | **Pokucha** |
| 30-49 | **Pokuchan** |
| 50 | Desbloqueo de **Mega-Pokuchan** |

Cada evolución modificará visualmente a la mascota y permitirá transmitir de forma clara el progreso realizado por el jugador.

Además de los cambios visuales, algunas evoluciones podrán desbloquear nuevos elementos de personalización, alimentos, decoraciones u otras recompensas.

# **Mega Evolución**

Al alcanzar el nivel 50, el jugador desbloqueará una nueva mecánica llamada **Mega Evolución**.

Cuando la barra de **Energía Mega Evolutiva** esté completamente cargada, el jugador podrá transformar temporalmente a Pokuchan en:

## **Mega-Pokuchan**

La Mega Evolución tendrá una duración máxima de 30 minutos. Durante este período, Mega-Pokuchan otorgará beneficios especiales al jugador cuando participe en minijuegos.

### **Bonificaciones de Mega-Pokuchan**

Mientras la Mega Evolución se encuentre activa:

* Los minijuegos otorgarán una bonificación de monedas.  
* La apariencia y animaciones de la mascota cambiarán temporalmente para representar su estado especial.

De esta forma, el jugador podrá decidir cuándo utilizar la transformación para aprovechar mejor sus recompensas.

# **Energía Mega Evolutiva**

La Mega Evolución contará con su propia barra de energía. Al activar a Mega-Pokuchan, esta energía comenzará a consumirse hasta finalizar después de aproximadamente 30 minutos de uso. Una vez agotada, Pokuchan regresará a su forma normal.

Para volver a utilizar la Mega Evolución, el jugador deberá regenerar la Energía Mega Evolutiva realizando actividades dentro del juego.

La energía podrá recuperarse mediante:

* Participar en minijuegos.  
* Alimentar a la mascota.  
* Asearla.  
* Jugar con ella.  
* Atender sus distintas necesidades.

De esta manera, la Mega Evolución funciona como una recompensa por jugar e interactuar activamente con la mascota, evitando que pueda utilizarse de manera permanente.

# **Gameplay Loop de la Mega Evolución**

El ciclo de esta mecánica será:

1. El jugador realiza actividades y minijuegos.  
2. Obtiene experiencia y progresa hasta alcanzar el nivel 50\.  
3. Desbloquea la Mega Evolución.  
4. Realiza actividades para cargar la barra de Energía Mega Evolutiva.  
5. Cuando la barra está completa, puede activar a Mega-Pokuchan.  
6. Mega-Pokuchan permanece activo durante un máximo de 30 minutos.  
7. Durante ese tiempo, el jugador obtiene bonificaciones de monedas y experiencia en los minijuegos.  
8. La Energía Mega Evolutiva se agota.  
9. La mascota regresa a su forma de Pokuchan.  
10. El jugador vuelve a realizar minijuegos y actividades de cuidado para regenerar la energía.

Este sistema genera un ciclo adicional de progresión y recompensa que incentiva al jugador a interactuar tanto con los minijuegos como con las mecánicas principales de cuidado de la mascota.

---

# **13\. Dirección artística**

El juego utilizará gráficos completamente **2D**. La dirección artística buscará transmitir una sensación alegre, amigable y divertida.

Se utilizarán principalmente:

* Colores vibrantes.  
* Formas redondeadas.  
* Personajes expresivos.  
* Animaciones exageradas y fáciles de interpretar.  
* Interfaces simples.  
* Botones grandes y claramente identificables.

La mascota deberá ser el principal foco visual del juego. Sus animaciones deberán permitir que el jugador pueda interpretar fácilmente su estado.

Por ejemplo, podrá reaccionar de manera diferente cuando tenga hambre, sueño, esté feliz o necesite higiene.

---

# **14\. Interfaz de usuario**

Debido a que el juego estará orientado principalmente a un público infantil, la interfaz deberá ser sencilla y visual. Los botones tendrán iconos fácilmente reconocibles.

En la interfaz principal se podrán visualizar los indicadores de:

* Hambre.  
* Higiene.  
* Diversión.  
* Energía.  
* Felicidad.

También se mostrará:

* Cantidad de monedas.  
* Cantidad de gemas.  
* Nivel del jugador.

Los elementos interactivos deberán ser suficientemente grandes para utilizarse cómodamente mediante pantalla táctil.

---

# **15\. Controles**

El juego estará diseñado exclusivamente alrededor de controles táctiles. Dependiendo de la actividad se utilizarán diferentes tipos de interacción:

**Tap:** seleccionar objetos, interactuar con la mascota o saltar en determinados minijuegos.

**Arrastrar:** mover objetos, alimentos o elementos de limpieza.

**Swipe:** desplazarse por menús o determinadas actividades.

**Dibujar:** utilizado principalmente en el sistema del cuadro y en algunos minijuegos.

El objetivo será evitar controles excesivamente complejos.

---

# **16\. Sonido y música**

La música tendrá un estilo alegre, tranquilo y amigable. Cada sección podrá contar con diferentes variantes musicales.

Por ejemplo:

* Música principal de la casa.  
* Música de tienda.  
* Música específica para minijuegos.

También habrá efectos de sonido para:

* Comprar objetos.  
* Obtener monedas.  
* Alimentar a la mascota.  
* Limpiarla.  
* Completar un nivel.  
* Perder una partida.  
* Seleccionar botones.  
* Interactuar con objetos.

La mascota también podrá producir diferentes sonidos para comunicar sus emociones.

---

# **17\. Ambientación**

El juego tendrá una ambientación amigable y fantástica. La mayor parte de la experiencia ocurrirá dentro de la casa de la mascota, aunque algunos minijuegos permitirán visitar escenarios adicionales.

Entre ellos podrán encontrarse:

* Bosques.  
* Playas.  
* Parques.  
* Ciudades.  
* Caminos.  
* Casas de amigos.

Estos escenarios permitirán aportar variedad visual sin abandonar la estética general del juego.

---

# **18\. Objetivo del jugador**

El objetivo general será cuidar a la mascota y mantenerla feliz mientras se desarrolla una relación progresiva con ella.

Paralelamente, el jugador podrá proponerse diferentes objetivos secundarios:

* Conseguir nuevas prendas.  
* Decorar completamente la casa.  
* Descubrir todas las recetas.  
* Obtener puntuaciones altas en los minijuegos.  
* Desbloquear nuevos escenarios.  
* Conseguir nuevas decoraciones.  
* Completar colecciones de objetos.

El juego no tendrá necesariamente un final tradicional, ya que estará diseñado como una experiencia continua.

---

# **19\. Diferenciales del juego**

Aunque toma como referencia juegos clásicos de mascotas virtuales, el proyecto buscará diferenciarse mediante la combinación de diferentes sistemas.

Entre sus principales características se encuentran:

* Cuidado de una mascota virtual.  
* Gran cantidad de opciones de personalización.  
* Varios minijuegos con mecánicas diferentes.  
* Sistema de recetas mediante combinación de ingredientes.  
* Actividades relacionadas con el cuidado del medioambiente.  
* Sistema de dibujo integrado dentro de la decoración de la casa.  
* Progresión y desbloqueo constante de contenido.

El cuadro personalizable será uno de los elementos distintivos, ya que permite que una creación realizada directamente por el jugador pase a formar parte de la decoración permanente de su casa virtual.

[image1]: <data:image/png;base64,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>