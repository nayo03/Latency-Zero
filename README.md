QUE HE HECHO PARA CREAR EL PROYECTO

- He instalado la versión unity 6.4, pq el urp no me que daba fallos
-UNNIVERSAL 3D urp -> según chatgpt es con lo que van las ar y vr, y es el rendering pipeline pr defecto en las nuevas versiones.

PROJECT SETINGS
	 [Editor]

(Edit → Project Settings → Editor)  esto lo he visto en un tutorial, pero en las nuevas versiones ya estaba por defecto
- Version Control → Visible Meta Files
- Asset Serialization → Force Text

	[Input system]

(Edit → Project Settings → Player → Other Settings)
- Active Input Handling → Both  (unity me dice que esta depreciado y recomienda cambiarlo)

PLUGINS INSTALADOS
- XR Plugin Management
- XR Interaction Toolkit
- AR Foundation
  
---
# 🧪 Setup del Proyecto (Prueba Inicial)

Sigue estos pasos para comprobar que el proyecto funciona correctamente en tu equipo.

---

## 📥 1. Clonar el repositorio

```bash
git clone https://github.com/nayo03/Latency-Zero
```

---

## 📂 2. Abrir el proyecto en Unity

1. Abre Unity Hub
2. Click en **Add → Add project from disk**
3. Selecciona la carpeta clonada (`Latency-Zero`)
4. Usa la versión EXACTA:

```
Unity 6000.4.0f1
```

⚠️ IMPORTANTE: todos debemos usar esta misma versión

---

## 📦 3. Instalación automática de paquetes

Al abrir el proyecto, Unity debería:

* Instalar dependencias automáticamente
* Configurar XR / AR

⏳ Puede tardar unos minutos la primera vez

---

## 🧪 4. Comprobaciones

Verifica que:

* No hay errores en la consola (Window → General → Console)
* El proyecto abre correctamente
* Ves las carpetas (`Assets`, `Packages`, etc.)

---

## ❗ 5. Posibles problemas

### 🔴 Faltan paquetes

* Espera a que Unity termine de cargar
* Reinicia Unity si es necesario

---

### 🔴 Errores raros

Prueba:

1. Cerrar Unity
2. Borrar carpeta:

```
Library/
```

3. Volver a abrir el proyecto

---

## ✅ 6. Si todo funciona

👉 El proyecto está correctamente configurado

Puedes avisar en el grupo 👍

---

## 🚀 Siguiente paso

Una vez validado por todos:

* Empezamos a trabajar en la estructura del proyecto
* Desarrollo de minijuegos

---
