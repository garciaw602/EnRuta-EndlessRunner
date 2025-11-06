# 🚀 ENRUTA - Endless Runner Cívico
**Desarrollado por [ PIXEL CÍVICO]**

---

### 💻 REQUERIMIENTOS TÉCNICOS
* **Motor de Juego:** Unity (6000.0.48f1).
* **Herramienta de Merge Recomendada:** **Visual Studio Code** (para la resolución manual de conflictos).

---

## ⚙️ ESTRUCTURA DE RAMAS (Flujo de Trabajo)

Esta tabla detalla el propósito de cada rama y sus reglas de integración.

| Rama | Propósito Principal | Regla de Bifurcación y Merge |
| :--- | :--- | :--- |
| **`main`** | **PRODUCCIÓN (Versión Estable)**. Lo que ve el público final. | Solo *merge* desde `dev` o **`hotfix`**. |
| **`dev`** | **INTEGRACIÓN DIARIA.** Versión de *testing* activa. | *Merge* desde ramas secundarias. |
| **`hotfix`** | **CORRECCIONES CRÍTICAS EN PRODUCCIÓN.** Para *bugs* urgentes en `main`. | **Bifurca de `main`. Mergea a `main` Y `dev` simultáneamente.** |
| **`wilson`, `juan-sebastian`** | TRABAJO CORE/LÓGICA. | *Merge* a `dev` vía Pull Request (PR). |
| **`ui-ux`** | INTERFACES Y LAYOUTS. | *Merge* a `dev` vía Pull Request (PR). |
| **`art-assets`** | MODELOS Y ESCALA 3D. | *Merge* a `dev` vía Pull Request (PR). |
| **`audio-assets`** | AUDIO Y SFX. | *Merge* a `dev` vía Pull Request (PR). |

---

### 🛠️ FLUJO DE TRABAJO DEL EQUIPO (Git Desktop & VS Code)

Esta tabla detalla las acciones y herramientas clave para el desarrollo diario y la integración de código.

| Flujo | Acción Principal | Herramienta Clave | Propósito del Paso |
| :--- | :--- | :--- | :--- |
| **Inicio Diario (Sincronización)** | Hacer **Pull/Fetch** desde la rama **`dev`** a la rama local **`dev`**. | Git Desktop | Asegurar que el desarrollador tiene la versión más reciente del trabajo del equipo. |
| **Desarrollo Diario** | Crear cambios, hacer **Commit** con mensajes descriptivos. | Git Desktop | Registrar el progreso local en la rama personal (`wilson`, etc.). |
| **Subir Cambios** | Hacer **Push** de la rama personal a GitHub. | Git Desktop | Compartir el código con el repositorio remoto, preparándose para el *merge*. |
| **Integración (PR)** | Crear el **Pull Request (PR)** de la rama personal a **`dev`**. | GitHub.com | Iniciar la revisión de código y la prueba de integración. |
| **Resolución de Conflictos** | Abrir archivos en conflicto y resolver manualmente las líneas de código. | Visual Studio Code | Herramienta esencial para resolver conflictos de *merge* de manera eficiente y precisa. |
| **Merge Final** | Aprobar y completar el *Merge* del PR a la rama **`dev`**. | GitHub.com | Integrar código estable y aprobado al tronco de desarrollo activo. |
| **Hotfix (Crítico)** | **Bifurcar de `main`** $\to$ Arreglar $\to$ **Merge a `main` y `dev`**. | GitHub.com / Git Desktop | Proceso de emergencia para parchar *bugs* en la versión de producción sin desestabilizar `dev`. |

---

### 🛑 REGLA CLAVE: PULL REQUESTS (PR)

**NUNCA HACER *PUSH* DIRECTO A `main` O `dev` DESDE UNA RAMA PERSONAL O DE EQUIPO.**

Todas las integraciones a `dev` deben hacerse a través de un **Pull Request (PR)** y deben ser aprobadas por el Líder de Proyecto/Core.


---



