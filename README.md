# Talkin Poku

Juego de mascota virtual 2D para Android, con minijuegos, tienda y personalizacion.

Trabajo practico integrador de **Desarrollo de videojuegos mobile** (UADE, 2do cuatrimestre 2026).

**Grupo 12:** Milena Janiot, Martina Dalbene, Renzo Bongiorno.

El diseño completo esta en [Grupo12_GDD.md](Grupo12_GDD.md).

## Requisitos

- Unity **6000.3.5f2** con el modulo *Android Build Support* (SDK, NDK y OpenJDK).
- Git.

## Como abrir

1. Clonar el repo.
2. Abrir la carpeta desde Unity Hub (`Add project from disk`).
3. La primera importacion tarda unos minutos porque genera `Library/`.

## Configuracion del proyecto

- Template **2D (URP)**, render pipeline Universal con 2D Renderer.
- Input System nuevo, con las acciones en `Assets/InputSystem_Actions.inputactions`.
- Orientacion fija en portrait.
- Android: IL2CPP, ARM64, min SDK 25, target SDK automatico.
- Package name `com.grupo12.talkinpoku`.

## Estructura

```
Assets/
  Scenes/          escenas del juego
  Settings/        assets de URP (UniversalRP, Renderer2D)
Packages/          manifest de paquetes
ProjectSettings/   configuracion del proyecto
Grupo12_GDD.md     documento de diseño
```

## Build

`File > Build Profiles > Android`. Para probar en el celular hace falta tener activada la
depuracion por USB y usar `Build And Run`.

Para las entregas se genera **APK** (instalacion directa), no AAB.
