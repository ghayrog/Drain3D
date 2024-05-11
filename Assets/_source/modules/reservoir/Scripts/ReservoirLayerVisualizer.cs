using UnityEngine;
using Surface;
using System.Threading.Tasks;

namespace Reservoir
{
    internal static class ReservoirLayerVisualizer
    {

        internal static Vector3 GetCellPosition(ReservoirLayer reservoirLayer, int i, int j)
        {
            float x = -(reservoirLayer.StructureGrid.Grid.GetLength(0) - 1) * reservoirLayer.StructureGrid.DX / 2 + (i + 0.5f) * reservoirLayer.StructureGrid.DX;
            float z = -(reservoirLayer.StructureGrid.Grid.GetLength(1) - 1) * reservoirLayer.StructureGrid.DY / 2 + (j + 0.5f) * reservoirLayer.StructureGrid.DY;
            float y = 0.25f * (reservoirLayer.StructureGrid.Grid[i, j] + reservoirLayer.StructureGrid.Grid[i+1, j] + reservoirLayer.StructureGrid.Grid[i, j+1] + reservoirLayer.StructureGrid.Grid[i+1, j+1]);
            return new Vector3(x, y, z);
        }

        internal static Mesh CreateReservoirMesh(ReservoirLayer reservoirLayer)
        {
            return MeshEditor.MakeMaskedMeshFromSurfaces(
                -(ReservoirLayer.GRID_INITIAL_SIZE - 1) * ReservoirLayer.CELL_INITIAL_SIZE / 2,
                0,
                -(ReservoirLayer.GRID_INITIAL_SIZE - 1) * ReservoirLayer.CELL_INITIAL_SIZE / 2,
                reservoirLayer.StructureGrid,
                reservoirLayer.StructureGrid.MaxValue,
                reservoirLayer.ThicknessGrid,
                reservoirLayer.VisibilityMask,
                reservoirLayer.FluidContact, true);
        }

        internal static async Task<Mesh> CreateReservoirMeshAsync(ReservoirLayer reservoirLayer)
        {
            MeshImpostor meshImpostor = await MeshImpostorEditor.MakeMaskedMeshFromSurfacesAsync(
                -(ReservoirLayer.GRID_INITIAL_SIZE - 1) * ReservoirLayer.CELL_INITIAL_SIZE / 2,
                0,
                -(ReservoirLayer.GRID_INITIAL_SIZE - 1) * ReservoirLayer.CELL_INITIAL_SIZE / 2,
                reservoirLayer.StructureGrid,
                reservoirLayer.StructureGrid.MaxValue,
                reservoirLayer.ThicknessGrid,
                reservoirLayer.VisibilityMask,
                reservoirLayer.FluidContact);
            return MeshImpostorEditor.ConvertImpostorToMesh(meshImpostor, true);
        }

        internal static IntersectionInfo GetRayIntersection(ReservoirLayer reservoir, Ray ray, Vector3 rayEnd, float baseDepth)
        {
            float length = (rayEnd - ray.origin).magnitude;
            int pointsNumber = (int)(length / (reservoir.StructureGrid.DX + reservoir.StructureGrid.DY) * 4);

            Vector3 gridOrigin = new Vector3(-(ReservoirLayer.GRID_INITIAL_SIZE - 1) * ReservoirLayer.CELL_INITIAL_SIZE / 2, baseDepth, -(ReservoirLayer.GRID_INITIAL_SIZE - 1) * ReservoirLayer.CELL_INITIAL_SIZE / 2);

            var returnValue = new IntersectionInfo();

            for (int k = 0; k <= pointsNumber; k++)
            {
                float t = (float)k / pointsNumber;
                float x = ray.origin.x + (rayEnd.x - ray.origin.x) * t;
                float y = ray.origin.y + (rayEnd.y - ray.origin.y) * t;
                float z = ray.origin.z + (rayEnd.z - ray.origin.z) * t;

                int i = (int)((x - gridOrigin.x) / reservoir.StructureGrid.DX);
                int j = (int)((z - gridOrigin.z) / reservoir.StructureGrid.DY);



                if (i < 0 || i >= reservoir.SizeX - 1 || j < 0 || j >= reservoir.SizeY - 1 || !reservoir.VisibilityMask[i, j])
                {
                    continue;
                }
                Vector3 vertex1 = new Vector3(gridOrigin.x + i * reservoir.StructureGrid.DX, reservoir.StructureGrid.Grid[i, j] + baseDepth, gridOrigin.z + j * reservoir.StructureGrid.DY);
                Vector3 vertex2 = new Vector3(gridOrigin.x + i * reservoir.StructureGrid.DX, reservoir.StructureGrid.Grid[i, j + 1] + baseDepth, gridOrigin.z + (j + 1) * reservoir.StructureGrid.DY);
                Vector3 vertex3 = new Vector3(gridOrigin.x + (i + 1) * reservoir.StructureGrid.DX, reservoir.StructureGrid.Grid[i + 1, j + 1] + baseDepth, gridOrigin.z + (j + 1) * reservoir.StructureGrid.DY);
                Vector3 vertex4 = new Vector3(gridOrigin.x + (i + 1) * reservoir.StructureGrid.DX, reservoir.StructureGrid.Grid[i + 1, j] + baseDepth, gridOrigin.z + j * reservoir.StructureGrid.DY);

                bool firstTriangleCheck = IntersectRayTriangle(ray, vertex1, vertex2, vertex3, false);
                bool secondTriangleCheck = IntersectRayTriangle(ray, vertex1, vertex3, vertex4, false);

                if (firstTriangleCheck || secondTriangleCheck)
                {
                    returnValue.isValid = true;
                    returnValue.i = i;
                    returnValue.j = j;
                    returnValue.x = gridOrigin.x + (i + 0.5f) * reservoir.StructureGrid.DX;
                    returnValue.y = baseDepth + (reservoir.StructureGrid.Grid[i, j] + reservoir.StructureGrid.Grid[i + 1, j] + reservoir.StructureGrid.Grid[i, j + 1] + reservoir.StructureGrid.Grid[i + 1, j + 1]) / 4;
                    returnValue.z = gridOrigin.z + (j + 0.5f) * reservoir.StructureGrid.DY;
                    return returnValue;
                }

            }
            returnValue.isValid = false;
            returnValue.i = -1;
            returnValue.j = -1;
            return returnValue;
        }

        internal static bool IntersectRayTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, bool bidirectional)
        {
            Vector3 ab = v1 - v0;
            Vector3 ac = v2 - v0;

            // Compute triangle normal. Can be precalculated or cached if
            // intersecting multiple segments against the same triangle
            Vector3 n = Vector3.Cross(ab, ac);

            // Compute denominator d. If d <= 0, segment is parallel to or points
            // away from triangle, so exit early
            float d = Vector3.Dot(-ray.direction, n);
            if (d <= 0.0f) return false;

            // Compute intersection t value of pq with plane of triangle. A ray
            // intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
            // dividing by d until intersection has been found to pierce triangle
            Vector3 ap = ray.origin - v0;
            float t = Vector3.Dot(ap, n);
            if ((t < 0.0f) && (!bidirectional)) return false;
            //if (t > d) return null; // For segment; exclude this code line for a ray test

            // Compute barycentric coordinate components and test if within bounds
            Vector3 e = Vector3.Cross(-ray.direction, ap);
            float v = Vector3.Dot(ac, e);
            if (v < 0.0f || v > d) return false;

            float w = -Vector3.Dot(ab, e);
            if (w < 0.0f || v + w > d) return false;

            return true;
        }
    }
}
