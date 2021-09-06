﻿
/***************************************************************************
*                                                                          *
*  Copyright (c) Raphaël Ernaelsten (@RaphErnaelsten)                      *
*  All Rights Reserved.                                                    *
*                                                                          *
*  NOTICE: Aura 2 is a commercial project.                                 * 
*  All information contained herein is, and remains the property of        *
*  Raphaël Ernaelsten.                                                     *
*  The intellectual and technical concepts contained herein are            *
*  proprietary to Raphaël Ernaelsten and are protected by copyright laws.  *
*  Dissemination of this information or reproduction of this material      *
*  is strictly forbidden.                                                  *
*                                                                          *
***************************************************************************/


using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aura2API
{
    /// <summary>
    /// Collection of static functions for drawing custom Gizmos in the Editor
    /// </summary>
    public static class CustomGizmo // TODO : INVESTIGATE WHY THIS HAS TO EXIST OUT OF EDITOR FOLDER
    {
#if UNITY_EDITOR
        #region Members
        /// <summary>
        /// Color of the lines
        /// </summary>
        public static Color color = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        /// <summary>
        /// Screen-space width of the lines
        /// </summary>
        public static float pixelWidth = 4.0f;
        /// <summary>
        /// Transparency factor when the pixel is behind a surface
        /// </summary>
        public const float occlusionOpacityFactor = 0.125f;
        /// <summary>
        /// Transparency color factor when the pixel is behind a surface
        /// </summary>
        public readonly static Color occlusionOpacityColorFactor = new Color(1.0f, 1.0f, 1.0f, occlusionOpacityFactor);
        /// <summary>
        /// The 2 pixels texture used to antialiase lines
        /// </summary>
        private static Texture2D _gizmoLineAaTexture;
        #endregion

        #region Properties
        /// <summary>
        /// The 2 pixels texture used to antialiase lines
        /// </summary>
        public static Texture2D GizmoLineAaTexture
        {
            get
            {
                if (_gizmoLineAaTexture == null)
                {
                    _gizmoLineAaTexture = new Texture2D(1, 2);
                    _gizmoLineAaTexture.SetPixels(new Color[] { new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f) });
                    _gizmoLineAaTexture.Apply();
                }

                return _gizmoLineAaTexture;
            }
        }
        #endregion

        #region Functions
        #region DrawLineSegment
        /// <summary>
        /// Draws a line segment between two points
        /// </summary>
        /// <param name="normalizedStartPosition">The position of the first point of the line segment, in normalized space</param>
        /// <param name="normalizedEndPosition">The position of the second point of the line segment, in normalized space</param>
        /// <param name="transform">The transformation matrix to be applied to the line segment</param>
        /// <param name="positionOffset">Position offset applied to the line segment, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the line segment, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the line segment, in local space</param>
        /// <param name="color">Color of the line segment</param>
        /// <param name="thickness">Thickness, in pixels, of the line segment</param>
        /// <param name="dotted">Draws a dotted line segment</param>
        public static void DrawLineSegment(Vector3 normalizedStartPosition, Vector3 normalizedEndPosition, Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness, bool dotted = false)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawLineSegment(normalizedStartPosition, normalizedEndPosition, transform, offsetMatrix, color, thickness, dotted);
        }
        /// <summary>
        /// Draws a line segment between two points
        /// </summary>
        /// <param name="normalizedStartPosition">The position of the first point of the line segment, in normalized space</param>
        /// <param name="normalizedEndPosition">The position of the second point of the line segment, in normalized space</param>
        /// <param name="transform">The transformation matrix to be applied to the line segment</param>
        /// <param name="offset">Offset matrix applied to the line segment, in local space</param>
        /// <param name="color">Color of the line segment</param>
        /// <param name="thickness">Thickness, in pixels, of the line segment</param>
        /// <param name="dotted">Draws a dotted line segment</param>
        public static void DrawLineSegment(Vector3 normalizedStartPosition, Vector3 normalizedEndPosition, Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness, bool dotted = false)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawLineSegment(normalizedStartPosition, normalizedEndPosition, offsetTransform, color, thickness, dotted);
        }
        /// <summary>
        /// Draws a line segment between two points
        /// </summary>
        /// <param name="absoluteStartPosition">The position of the first point of the line segment, in world space</param>
        /// <param name="absoluteEndPosition">The position of the second point of the line segment, in world space</param>
        /// <param name="color">Color of the line segment</param>
        /// <param name="thickness">Thickness, in pixels, of the line segment</param>
        /// <param name="dotted">Draws a dotted line segment</param>
        public static void DrawLineSegment(Vector3 absoluteStartPosition, Vector3 absoluteEndPosition, Color color, float thickness, bool dotted = false)
        {
            if(absoluteStartPosition != absoluteEndPosition)
            {
                Vector3 position = (absoluteStartPosition + absoluteEndPosition) * 0.5f;
                Quaternion rotation = Quaternion.LookRotation((absoluteEndPosition - absoluteStartPosition).normalized);
                Vector3 scale = Vector3.one * Vector3.Distance(absoluteStartPosition, absoluteEndPosition) * 0.5f;

                DrawLineSegment(Vector3.back, Vector3.forward, Matrix4x4.TRS(position, rotation, scale), color, thickness, dotted);
            }
        }
        /// <summary>
        /// Draws a line segment between two points
        /// </summary>
        /// <param name="normalizedStartPosition">The position of the first point of the line segment, in normalized space</param>
        /// <param name="normalizedEndPosition">The position of the second point of the line segment, in normalized space</param>
        /// <param name="transform">The transformation matrix to be applied to the line segment</param>
        /// <param name="color">Color of the line segment</param>
        /// <param name="thickness">Thickness, in pixels, of the line segment</param>
        /// <param name="dotted">Draws a dotted line segment</param>
        public static void DrawLineSegment(Vector3 normalizedStartPosition, Vector3 normalizedEndPosition, Matrix4x4 transform, Color color, float thickness, bool dotted = false)
        {
            Vector3[] points = new Vector3[2];
            points[0] = transform.MultiplyPoint(normalizedStartPosition); ;
            points[1] = transform.MultiplyPoint(normalizedEndPosition);

            Color tmp = Handles.color;
            Handles.color = color * occlusionOpacityColorFactor;
            // Draws the gizmo only if depth > pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawLineSegment(points, thickness, dotted);
            Handles.color = color;
            // Then draws the gizmo only if depth <= pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawLineSegment(points, thickness, dotted);
            Handles.color = tmp;
        }
        /// <summary>
        /// Draws a line segment between two points (no zTest)
        /// </summary>
        /// <param name="points">The position of the points of the line segment, in world space</param>
        /// <param name="absoluteEndPosition">The position of the second point of the line segment, in world space</param>
        /// <param name="thickness">Thickness, in pixels, of the line segment</param>
        /// <param name="dotted">Draws a dotted line segment</param>
        private static void DrawLineSegment(Vector3[] points, float thickness, bool dotted)
        {
            if (dotted)
            {
                Handles.DrawDottedLine(points[0], points[1], thickness); // no AA texture support for dotted lines
            }
            else
            {
                Handles.DrawAAPolyLine(GizmoLineAaTexture, thickness, points);
            }
        }
        #endregion

        #region DrawSquare
        /*
                            D____________C
                           /		    /
                          /			   /	
                         /  	      /  
                        /		     /	
                       /	        /	
                      A____________B

        */
        public static Vector3 squareCornerA
        {
            get
            {
                return new Vector3(-0.5f, 0.0f, -0.5f);
            }
        }
        public static Vector3 squareCornerB
        {
            get
            {
                return new Vector3(0.5f, 0.0f, -0.5f);
            }
        }
        public static Vector3 squareCornerC
        {
            get
            {
                return new Vector3(0.5f, 0.0f, 0.5f);
            }
        }
        public static Vector3 squareCornerD
        {
            get
            {
                return new Vector3(-0.5f, 0.0f, 0.5f);
            }
        }
        
        public static void DrawSquare(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawSquare(transform, offsetMatrix, color, thickness);
        }
        public static void DrawSquare(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawSquare(offsetTransform, color, thickness);
        }
        public static void DrawSquare(Matrix4x4 transform, Color color, float thickness)
        {
            // a -> b
            DrawLineSegment(squareCornerA, squareCornerB, transform, color, thickness);
            // b -> c
            DrawLineSegment(squareCornerB, squareCornerC, transform, color, thickness);
            // c -> d
            DrawLineSegment(squareCornerC, squareCornerD, transform, color, thickness);
            // d -> a
            DrawLineSegment(squareCornerD, squareCornerA, transform, color, thickness);
        }
        #endregion

        #region DrawCube
        /*
                            E_______________F
                           /|		    _/	|
                          /	|		  _/	|
                         /  |	    _/		|	FAR
                        /	|	  _/		|
                       /	H____/__________G
                      A____/__B        __/
                      |	 _/	  |     __/
            NEAR	  |_/	  |  __/
                      D_______C_/

        */
        public static Vector3 cubeCornerA
        {
            get
            {
                return new Vector3(-0.5f, 0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerB
        {
            get
            {
                return new Vector3(0.5f, 0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerC
        {
            get
            {
                return new Vector3(0.5f, -0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerD
        {
            get
            {
                return new Vector3(-0.5f, -0.5f, -0.5f);
            }
        }
        public static Vector3 cubeCornerE
        {
            get
            {
                return new Vector3(-0.5f, 0.5f, 0.5f);
            }
        }
        public static Vector3 cubeCornerF
        {
            get
            {
                return new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
        public static Vector3 cubeCornerG
        {
            get
            {
                return new Vector3(0.5f, -0.5f, 0.5f);
            }
        }
        public static Vector3 cubeCornerH
        {
            get
            {
                return new Vector3(-0.5f, -0.5f, 0.5f);
            }
        }

        /// <summary>
        /// Draws a cube
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cube</param>
        /// <param name="positionOffset">Position offset applied to the cube, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the cube, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the cube, in local space</param>
        /// <param name="color">Color of the cube</param>
        /// <param name="thickness">Thickness, in pixels, of the cube edges</param>
        public static void DrawCube(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawCube(transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a cube
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cube</param>
        /// <param name="offset">Offset matrix applied to the cube, in local space</param>
        /// <param name="color">Color of the cube</param>
        /// <param name="thickness">Thickness, in pixels, of the cube edges</param>
        public static void DrawCube(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawCube(offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a cube
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cube</param>
        /// <param name="color">Color of the cube</param>
        /// <param name="thickness">Thickness, in pixels, of the cube edges</param>
        public static void DrawCube(Matrix4x4 transform, Color color, float thickness)
        {
            // a -> b
            DrawLineSegment(cubeCornerA, cubeCornerB, transform, color, thickness);
            // b -> c
            DrawLineSegment(cubeCornerB, cubeCornerC, transform, color, thickness);
            // c -> d
            DrawLineSegment(cubeCornerC, cubeCornerD, transform, color, thickness);
            // d -> a
            DrawLineSegment(cubeCornerD, cubeCornerA, transform, color, thickness);

            // e -> f
            DrawLineSegment(cubeCornerE, cubeCornerF, transform, color, thickness);
            // f -> g
            DrawLineSegment(cubeCornerF, cubeCornerG, transform, color, thickness);
            // g -> h
            DrawLineSegment(cubeCornerG, cubeCornerH, transform, color, thickness);
            // h -> e
            DrawLineSegment(cubeCornerH, cubeCornerE, transform, color, thickness);

            // a -> e
            DrawLineSegment(cubeCornerA, cubeCornerE, transform, color, thickness);
            // b -> f
            DrawLineSegment(cubeCornerB, cubeCornerF, transform, color, thickness);
            // c -> g
            DrawLineSegment(cubeCornerC, cubeCornerG, transform, color, thickness);
            // d -> h
            DrawLineSegment(cubeCornerD, cubeCornerH, transform, color, thickness);
        }
        #endregion

        #region DrawBezier
        /// <summary>
        /// Draws a Bézier curve
        /// </summary>
        /// <param name="startPosition">The position of the first point of the Bézier curve, in normalized space</param>
        /// <param name="startTangent">The position of the tangent of the first point of the Bézier curve, in normalized space</param>
        /// <param name="endPosition">The position of the second point of the Bézier curve, in normalized space</param>
        /// <param name="endTangent">The position of the tangent of the second point of the Bézier curve, in normalized space</param>
        /// <param name="transform">The transformation matrix to be applied to the Bézier curve</param>
        /// <param name="positionOffset">Position offset applied to the Bézier curve, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the Bézier curve, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the Bézier curve, in local space</param>
        /// <param name="color">Color of the Bézier curve</param>
        /// <param name="thickness">Thickness, in pixels, of the Bézier curve</param>
        public static void DrawBezier(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawBezier(startPosition, startTangent, endPosition, endTangent, transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a Bézier curve
        /// </summary>
        /// <param name="startPosition">The position of the first point of the Bézier curve, in normalized space</param>
        /// <param name="startTangent">The position of the tangent of the first point of the Bézier curve, in normalized space</param>
        /// <param name="endPosition">The position of the second point of the Bézier curve, in normalized space</param>
        /// <param name="endTangent">The position of the tangent of the second point of the Bézier curve, in normalized space</param>
        /// <param name="transform">The transformation matrix to be applied to the Bézier curve</param>
        /// <param name="offset">Offset matrix applied to the Bézier curve, in local space</param>
        /// <param name="color">Color of the Bézier curve</param>
        /// <param name="thickness">Thickness, in pixels, of the Bézier curve</param>
        public static void DrawBezier(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawBezier(startPosition, startTangent, endPosition, endTangent, offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a Bézier curve
        /// </summary>
        /// <param name="startPosition">The position of the first point of the Bézier curve, in normalized space</param>
        /// <param name="startTangent">The position of the tangent of the first point of the Bézier curve, in normalized space</param>
        /// <param name="endPosition">The position of the second point of the Bézier curve, in normalized space</param>
        /// <param name="endTangent">The position of the tangent of the second point of the Bézier curve, in normalized space</param>
        /// <param name="transform">The transformation matrix to be applied to the Bézier curve</param>
        /// <param name="color">Color of the Bézier curve</param>
        /// <param name="thickness">Thickness, in pixels, of the Bézier curve</param>
        public static void DrawBezier(Vector3 startPosition, Vector3 startTangent, Vector3 endPosition, Vector3 endTangent, Matrix4x4 transform, Color color, float thickness)
        {
            startPosition = transform.MultiplyPoint(startPosition);
            startTangent = transform.MultiplyPoint(startTangent);
            endPosition = transform.MultiplyPoint(endPosition);
            endTangent = transform.MultiplyPoint(endTangent);
            
            // Draws the gizmo only if depth > pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color * occlusionOpacityColorFactor, GizmoLineAaTexture, thickness);
            // Then draws the gizmo only if depth <= pixel's
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, GizmoLineAaTexture, thickness);
        }
        #endregion

        #region DrawCircle
        const float circleTangentCoeficient = 0.551915024494f;

        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the circle</param>
        /// <param name="positionOffset">Position offset applied to the circle, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the circle, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the circle, in local space</param>
        /// <param name="color">Color of the circle</param>
        /// <param name="thickness">Thickness, in pixels, of the circle</param>
        public static void DrawCircle(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawCircle(transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the circle</param>
        /// <param name="offset">Offset matrix applied to the circle, in local space</param>
        /// <param name="color">Color of the circle</param>
        /// <param name="thickness">Thickness, in pixels, of the circle</param>
        public static void DrawCircle(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawCircle(offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a circle
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the circle</param>
        /// <param name="color">Color of the circle</param>
        /// <param name="thickness">Thickness, in pixels, of the circle</param>
        public static void DrawCircle(Matrix4x4 transform, Color color, float thickness)
        {
            DrawBezier(Vector3.right, Vector3.right + Vector3.forward * circleTangentCoeficient, Vector3.forward, Vector3.forward + Vector3.right * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.forward, Vector3.forward - Vector3.right * circleTangentCoeficient, Vector3.left, Vector3.left + Vector3.forward * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.left, Vector3.left + Vector3.back * circleTangentCoeficient, Vector3.back, Vector3.back - Vector3.right * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
            DrawBezier(Vector3.back, Vector3.back + Vector3.right * circleTangentCoeficient, Vector3.right, Vector3.right + Vector3.back * circleTangentCoeficient, transform, Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 0.5f), color, thickness);
        }
        #endregion

        #region DrawSphere
        /// <summary>
        /// Draws a sphere
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the sphere</param>
        /// <param name="positionOffset">Position offset applied to the sphere, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the sphere, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the sphere, in local space</param>
        /// <param name="color">Color of the sphere</param>
        /// <param name="thickness">Thickness, in pixels, of the sphere</param>
        public static void DrawSphere(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawSphere(transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a sphere
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the sphere</param>
        /// <param name="offset">Offset matrix applied to the sphere, in local space</param>
        /// <param name="color">Color of the sphere</param>
        /// <param name="thickness">Thickness, in pixels, of the sphere</param>
        public static void DrawSphere(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawSphere(offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a sphere
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the sphere</param>
        /// <param name="color">Color of the sphere</param>
        /// <param name="thickness">Thickness, in pixels, of the sphere</param>
        public static void DrawSphere(Matrix4x4 transform, Color color, float thickness)
        {
            DrawCircle(transform, color, thickness);
            DrawCircle(transform, Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.right), Vector3.one), color, thickness);
            DrawCircle(transform, Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(90, Vector3.forward), Vector3.one), color, thickness);
        }
        #endregion

        #region DrawCylinder
        /// <summary>
        /// Draws a cylinder
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cylinder</param>
        /// <param name="positionOffset">Position offset applied to the cylinder, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the cylinder, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the cylinder, in local space</param>
        /// <param name="color">Color of the cylinder</param>
        /// <param name="thickness">Thickness, in pixels, of the cylinder</param>
        public static void DrawCylinder(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawCylinder(transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a cylinder
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cylinder</param>
        /// <param name="offset">Offset matrix applied to the cylinder, in local space</param>
        /// <param name="color">Color of the cylinder</param>
        /// <param name="thickness">Thickness, in pixels, of the cylinder</param>
        public static void DrawCylinder(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawCylinder(offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a cylinder
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cylinder</param>
        /// <param name="color">Color of the cylinder</param>
        /// <param name="thickness">Thickness, in pixels, of the cylinder</param>
        public static void DrawCylinder(Matrix4x4 transform, Color color, float thickness)
        {
            DrawCircle(transform, Matrix4x4.TRS(Vector3.up * 0.5f, Quaternion.identity, Vector3.one), color, thickness);
            DrawLineSegment(new Vector3(0.0f, 0.5f, 0.5f), new Vector3(0.0f, -0.5f, 0.5f), transform, color, thickness);
            DrawLineSegment(new Vector3(0.0f, 0.5f, -0.5f), new Vector3(0.0f, -0.5f, -0.5f), transform, color, thickness);
            DrawLineSegment(new Vector3(0.5f, 0.5f, 0.0f), new Vector3(0.5f, -0.5f, 0.0f), transform, color, thickness);
            DrawLineSegment(new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(-0.5f, -0.5f, 0.0f), transform, color, thickness);
            DrawCircle(transform, Matrix4x4.TRS(Vector3.down * 0.5f, Quaternion.identity, Vector3.one), color, thickness);
        }
        #endregion

        #region DrawCone
        /// <summary>
        /// Draws a cone
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cone</param>
        /// <param name="positionOffset">Position offset applied to the cone, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the cone, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the cone, in local space</param>
        /// <param name="color">Color of the cone</param>
        /// <param name="thickness">Thickness, in pixels, of the cone</param>
        public static void DrawCone(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawCone(transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a cone
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cone</param>
        /// <param name="offset">Offset matrix applied to the cone, in local space</param>
        /// <param name="color">Color of the cone</param>
        /// <param name="thickness">Thickness, in pixels, of the cone</param>
        public static void DrawCone(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawCone(offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a cone
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cone</param>
        /// <param name="color">Color of the cone</param>
        /// <param name="thickness">Thickness, in pixels, of the cone</param>
        public static void DrawCone(Matrix4x4 transform, Color color, float thickness)
        {
            DrawLineSegment(Vector3.zero, new Vector3(0, 0.5f, 1.0f), transform, color, thickness);
            DrawLineSegment(Vector3.zero, new Vector3(0, -0.5f, 1.0f), transform, color, thickness);
            DrawLineSegment(Vector3.zero, new Vector3(0.5f, 0.0f, 1.0f), transform, color, thickness);
            DrawLineSegment(Vector3.zero, new Vector3(-0.5f, 0.0f, 1.0f), transform, color, thickness);
            DrawCircle(transform, Matrix4x4.TRS(Vector3.forward, Quaternion.AngleAxis(90, Vector3.right), Vector3.one), color, thickness);
        }
        #endregion

        #region DrawCross
        /// <summary>
        /// Draws a 3D/six-axis cross
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cross</param>
        /// <param name="positionOffset">Position offset applied to the cross, in local space</param>
        /// <param name="rotationOffset">Rotation offset applied to the cross, in local space</param>
        /// <param name="scaleOffset">Scale offset applied to the cross, in local space</param>
        /// <param name="color">Color of the cross</param>
        /// <param name="thickness">Thickness, in pixels, of the cross</param>
        public static void DrawCross(Matrix4x4 transform, Vector3 positionOffset, Quaternion rotationOffset, Vector3 scaleOffset, Color color, float thickness)
        {
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(positionOffset, rotationOffset, scaleOffset);
            DrawCross(transform, offsetMatrix, color, thickness);
        }
        /// <summary>
        /// Draws a 3D/six-axis cross
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cross</param>
        /// <param name="offset">Offset matrix applied to the cross, in local space</param>
        /// <param name="color">Color of the cross</param>
        /// <param name="thickness">Thickness, in pixels, of the cone</param>
        public static void DrawCross(Matrix4x4 transform, Matrix4x4 offset, Color color, float thickness)
        {
            Matrix4x4 offsetTransform = transform * offset;
            DrawCross(offsetTransform, color, thickness);
        }
        /// <summary>
        /// Draws a 3D/six-axis cross
        /// </summary>
        /// <param name="transform">The transformation matrix to be applied to the cross</param>
        /// <param name="color">Color of the cross</param>
        /// <param name="thickness">Thickness, in pixels, of the cross</param>
        public static void DrawCross(Matrix4x4 transform, Color color, float thickness)
        {
            DrawLineSegment(-Vector3.right * 0.5f, Vector3.right * 0.5f, transform, color, thickness);
            DrawLineSegment(-Vector3.up * 0.5f, Vector3.up * 0.5f, transform, color, thickness);
            DrawLineSegment(-Vector3.forward * 0.5f, Vector3.forward * 0.5f, transform, color, thickness);
        }
        #endregion

        #region Label
        /// <summary>
        /// Draws a label on screen right next to the mouse
        /// </summary>
        /// <param name="currentEvent">The current Event. You can use Event.current if usure.</param>
        /// <param name="text">The text of the label</param>
        /// <param name="color">The color of the label</param>
        /// <param name="style">The style to draw the label with</param>
        public static void DrawLabelNextToMouse(Event currentEvent, string text, Color color, Vector2 pixelsOffset, GUIStyle style = null)
        {
            DrawLabel(currentEvent.mousePosition + pixelsOffset, text, color, style);
        }
        /// <summary>
        /// Draws a label on screen
        /// </summary>
        /// <param name="screenSpacePosition">The screenspace position of the label</param>
        /// <param name="text">The text of the label</param>
        /// <param name="color">The color of the label</param>
        /// <param name="style">The style to draw the label with</param>
        public static void DrawLabel(Vector2 screenSpacePosition, string text, Color color, GUIStyle style = null)
        {
            DrawLabel(HandleUtility.GUIPointToWorldRay(screenSpacePosition).origin, text, color, style);
        }
        /// <summary>
        /// Draws a label on screen
        /// </summary>
        /// <param name="worldSpacePosition">The world position of the label</param>
        /// <param name="text">The text of the label</param>
        /// <param name="color">The color of the label</param>
        /// <param name="style">The style to draw the label with</param>
        public static void DrawLabel(Vector3 worldSpacePosition, string text, Color color, GUIStyle style = null)
        {
            Color tmp = Handles.color;
            Handles.color = color;
            Handles.Label(worldSpacePosition, text, style);
            Handles.color = tmp;
        }
        #endregion

        #region Grid
        /// <summary>
        /// Draws a grid
        /// </summary>
        /// <param name="matrix">The matrix of the grid</param>
        /// <param name="size">The size of the grid</param>
        /// <param name="cellsAmount">The amout of cells</param>
        /// <param name="subdivisions">The amount of subcellss</param>
        /// <param name="mainCellsColor">The main cells lines color</param>
        /// <param name="mainCellsThickness">The main cells lines thickness</param>
        /// <param name="subCellsColor">The subcells lines color</param>
        /// <param name="subCellsThickness">The subcells lines thickness</param>
        /// <param name="dottedSubCells">Draw subcell lines as dotted lines</param>
        public static void DrawGrid(Matrix4x4 matrix, float size, int cellsAmount, int subdivisions, Color mainCellsColor, float mainCellsThickness, Color subCellsColor, float subCellsThickness, bool dottedSubCells = false)
        {
            int cellsAmountPerSide = cellsAmount * subdivisions;
            for (int i = -cellsAmountPerSide; i <= cellsAmountPerSide; ++i)
            {
                float ratio = (float)i / (float)cellsAmountPerSide;
                bool isMainCell = i % subdivisions == 0;
                if(isMainCell)
                {
                    DrawLineSegment(new Vector3(ratio, 0, 1) * size, new Vector3(ratio, 0, -1) * size, matrix, mainCellsColor, mainCellsThickness);
                    DrawLineSegment(new Vector3(1, 0, ratio) * size, new Vector3(-1, 0, ratio) * size, matrix, mainCellsColor, mainCellsThickness);
                }
                else
                {
                    DrawLineSegment(new Vector3(ratio, 0, 1) * size, new Vector3(ratio, 0, -1) * size, matrix, subCellsColor, subCellsThickness, dottedSubCells);
                    DrawLineSegment(new Vector3(1, 0, ratio) * size, new Vector3(-1, 0, ratio) * size, matrix, subCellsColor, subCellsThickness, dottedSubCells);
                }
            }
        }
        #endregion
        #endregion
#endif
    }
}
