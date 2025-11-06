# 🚀 EN RUTA - Endless Runner Cívico
**Desarrollado por [Nombre del Estudio, ej: PIXEL CÍVICO]**

### ⚙️ ESTRUCTURA DE RAMAS (Flujo de Trabajo)

| Rama | Propósito Principal | Regla General |
| :--- | :--- | :--- |
| **`main`** | **PRODUCCIÓN (Versión Estable).** Contiene solo código probado, listo para *build* o demo. | Solo *merge* desde `dev`. |
| **`dev`** | **INTEGRACIÓN DIARIA.** Rama central donde se unen todas las *features* probadas localmente. Versión de *testing* activa. | *Merge* desde ramas secundarias (`wilson`, `art-assets`, `ui-ux`). |
| **`wilson`, `juan-sebastian`** | **TRABAJO CORE/LÓGICA.** Desarrollo diario del *Core* del juego. | *Merge* a `dev` vía Pull Request (PR). |
| **`ui-ux`** | **INTERFACES Y LAYOUTS.** Implementación de la UI, menús y la Ventana de Clasificación. | *Merge* a `dev` vía PR. |
| **`art-assets`** | **MODELOS Y ESCALA 3D.** Importación de *chunks*, edificios y *assets*. | *Merge* a `dev` vía PR. |

### 🛑 REGLA CLAVE: PULL REQUESTS (PR)

**NUNCA HACER *PUSH* DIRECTO A `main` O `dev` DESDE UNA RAMA PERSONAL O DE EQUIPO.**

Todas las integraciones a `dev` deben hacerse a través de un **Pull Request (PR)** y deben ser aprobadas por el Líder de Proyecto/Core.

---

### 💻 REQUERIMIENTOS TÉCNICOS
* **Motor de Juego:** Unity (6000.0.48f1).
* **Herramienta de Merge Recomendada:** **Visual Studio Code** (para la resolución manual de conflictos).
