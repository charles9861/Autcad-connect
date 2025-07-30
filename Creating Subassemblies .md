Absolutely — creating custom **Subassemblies** in **Autodesk Civil 3D** can be done either using the **Subassembly Composer (SAC)** or programmatically using the **.NET API**. Both methods have distinct advantages, and as a Civil 3D expert developer, here's a **detailed explanation**, including **use cases, technical differences, and a complete example using .NET**.

---

## ✅ Overview: What is a Subassembly in Civil 3D?

A **Subassembly** defines a building block of a **Corridor**, such as lanes, shoulders, curbs, sidewalks, or ditches. Each subassembly contains **geometry, behavior, input parameters**, and **codes** used for corridor modeling and quantity takeoffs.

---

## 🔧 1. Creating Subassemblies: Two Approaches

### 🧱 A. Subassembly Composer (SAC)

**Visual tool** to create subassemblies without writing code.

**Pros:**

* Drag-and-drop interface
* Easier for designers with no programming skills
* Faster prototyping
* Built-in test interface

**Cons:**

* Limited logic control
* No access to external data sources
* Difficult to integrate with external databases or complex rules
* No looping/recursive logic or external event hooks

---

### 👨‍💻 B. .NET API (Custom .NET Subassemblies)

Using **C# or VB.NET**, you can build compiled subassemblies as .DLLs.

**Pros:**

* Full control over geometry and logic
* Use external APIs, databases, configuration files
* Implement complex behaviors (conditional logic, iteration, data validation)
* Easily version-controlled and distributed via deployment tools

**Cons:**

* Requires knowledge of .NET programming
* No visual interface to preview or test logic (must be loaded in Civil 3D)

---

## 🔨 2. Creating Subassemblies with .NET

### 🚧 Step-by-Step Example

Let’s create a simple **Lane with Width and Slope** using .NET:

### 📁 File Structure

* Visual Studio Class Library (.NET Framework 4.8)
* Reference: `AeccDbMgd.dll`, `AeccRoadwayMgd.dll`, `AcMgd.dll`, `AcDbMgd.dll` from Civil 3D installation path.

---

### 📜 Code: SimpleLaneSubassembly.cs

```csharp
using System;
using Autodesk.Civil.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

[assembly: ExtensionApplication(typeof(MySubassembly.SimpleLaneSubassembly))]

namespace MySubassembly
{
    public class SimpleLaneSubassembly : Subassembly
    {
        public override void Init()
        {
            this.Name = "SimpleLane";
            this.DisplayName = "Simple Lane with Width and Slope";

            // Input parameters
            this.InputParameters.AddDouble("LaneWidth", "Width of the lane", 3.5);
            this.InputParameters.AddDouble("LaneSlope", "Slope of the lane (in %)", -2.0);
        }

        public override void ComputePoints()
        {
            // Read inputs
            double width = this.InputParameters["LaneWidth"];
            double slope = this.InputParameters["LaneSlope"] / 100.0;

            // Define the point (offset, elevation) relative to baseline
            Point2d start = new Point2d(0, 0);
            Point2d end = new Point2d(width, width * slope);

            // Add geometry
            this.OutputPoints.Add("P1", start);
            this.OutputPoints.Add("P2", end);

            this.OutputLinks.Add("Lane", "P1", "P2");

            // Add codes (for QTO/point identification)
            this.PointCodes["P1"].Add("InsideEdge");
            this.PointCodes["P2"].Add("OutsideEdge");

            this.LinkCodes["Lane"].Add("Pavement");
        }
    }
}
```

---

### 🔧 Build & Load

1. Build the DLL in Visual Studio.
2. Load it into Civil 3D via `NETLOAD`.
3. It will appear as a subassembly in the Tool Palette > Miscellaneous.

---

## 🧠 Advanced Features with .NET (Beyond SAC Capabilities)

| Feature                                                                  | SAC        | .NET |
| ------------------------------------------------------------------------ | ---------- | ---- |
| Custom external data (e.g., soil map, GIS DB)                            | ❌          | ✅    |
| Looping logic (e.g., repeating structures)                               | ❌          | ✅    |
| Conditional geometry (multi-stage behavior)                              | ⚠️ Limited | ✅    |
| Integration with company-specific rules (e.g., traffic class, materials) | ❌          | ✅    |
| Direct access to AutoCAD or Civil 3D database                            | ❌          | ✅    |

---

## 🧪 Example Use Cases for .NET

1. **Bridge Decking**: Automatically adjust cross-slope based on span length.
2. **Utility Trenches**: Variable trench width and protection based on pipe type.
3. **Geotechnical Cut/Fill**: Adjust ditch or benching logic based on strata from external DB.
4. **Smart Lanes**: Connect to traffic API to model high-use areas with thicker pavement.

---

## 📦 Deployment

* Create an installer (.msi) or use a shared `bundle` folder:

```text
Civil 3D Bundle Structure:
\MySubassembly.bundle\
    \Contents\
        MySubassembly.dll
    PackageContents.xml
```

```xml
<AutodeskProduct>
  <Components>
    <ComponentEntry AppName="MySubassembly" ModuleName="./Contents/MySubassembly.dll" LoadOnAutoCADStartup="True" />
  </Components>
</AutodeskProduct>
```

---

## ✅ When to Use What?

| Situation                                          | Use SAC | Use .NET |
| -------------------------------------------------- | ------- | -------- |
| Simple geometry, quick turnaround                  | ✅       | ❌        |
| Dynamic or rule-based logic                        | ❌       | ✅        |
| External data sources or rulesets                  | ❌       | ✅        |
| Team of designers, no programmers                  | ✅       | ❌        |
| Full automation or integration with design systems | ❌       | ✅        |

---

## ✅ Conclusion

* Use **Subassembly Composer** for **quick, simple, visual tasks**.
* Use **.NET API** for **scalable, logic-rich, data-integrated subassemblies**.

Both can coexist — you might prototype in SAC and rewrite in .NET for production workflows.

---

Would you like a more complex example (e.g., ditch with daylight targeting, retaining wall, or bench cut logic)?
