import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import AuthLayout from "@/layouts/AuthLayout/AuthLayout";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import Login from "@/pages/Login/Login";
import UserManagement from "@/pages/Principal/UserManagement";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import GradeBatch from "@/pages/Teacher/GradeBatch/GradeBatch";
import MarkReportTable from "@/pages/Teacher/MarkReport/MarkReportTable";
import AddTeacher from "@/pages/Teacher/Profile/AddTeacher";
import ProtectedRoute from "@/routes/ProtectedRoute";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, RouterProvider } from "react-router";
import AuthRedirectRoute from "./AuthRedirectRoute";

const TeacherTable = lazy(() => import("@/pages/Teacher/Profile/TeacherTable"));
const StudentTable = lazy(() => import("@/pages/Student/Profile/StudentTable"));
const TeacherProfile = lazy(
  () => import("@/pages/Teacher/Profile/TeacherProfile"),
);
const StudentProfile = lazy(
  () => import("@/pages/Student/Profile/StudentProfile"),
);
const TATable = lazy(
  () => import("@/pages/Teacher/TeachingAssignment/TATable"),
);
const HTATable = lazy(
  () => import("@/pages/Teacher/HeadTeacherAssignment/HTATable"),
);

const StudentScore = lazy(
  () => import("@/pages/Student/SummaryScore/StudentScore"),
);

const AppRouter = () => {
  const routes = [...privateRouter, ...authRoutes];
  // const routes = authRoutes;

  const router = createBrowserRouter([...routes]);

  return <RouterProvider router={router} />;
};

const authRoutes = [
  {
    element: (
      <AuthRedirectRoute>
        <AuthLayout />
      </AuthRedirectRoute>
    ),
    children: [
      {
        path: "/login",
        element: <Login />,
      },
    ],
  },
];

const adminRouter = [
  {
    path: "/admin/user",
    element: <UserManagement />,
  },
];

const teacherRouter = [
  {
    path: "/teacher/profile",
    element: (
      <ProtectedRoute requiredRoles={["Principal"]}>
        <ErrorBoundary fallback={<FallbackErrorBoundary />}>
          <Suspense fallback={<div>Loading...</div>}>
            <TeacherTable />
          </Suspense>
        </ErrorBoundary>
      </ProtectedRoute>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/profile/:id",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherProfile />
        </Suspense>
      </ErrorBoundary>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/profile/create-teacher",
    element: (
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <AddTeacher />
      </ErrorBoundary>
    ),
    // errorElement: <ErrorRouteComponent />,
  },
  {
    path: "/teacher/teaching-assignment",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <TATable />
      </Suspense>
    ),
  },
  {
    path: "teacher/take-attendance",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <AttendanceTable />
      </Suspense>
    ),
  },
  {
    path: "/teacher/head-teacher-assignment",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <HTATable />
      </Suspense>
    ),
  },
  {
    path: "/teacher/mark-report",
    element: (
      <ProtectedRoute requiredRoles={["Teacher"]}>
        <Suspense fallback={<div>Loading...</div>}>
          <MarkReportTable />
        </Suspense>
      </ProtectedRoute>
    ),
  },
  {
    path: "/teacher/grade-batch",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <GradeBatch />
      </Suspense>
    ),
  },
];

const studentRouter = [
  {
    path: "/student/profile",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <StudentTable />
      </Suspense>
    ),
  },
  {
    path: "/student/profile/:id",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <StudentProfile />
      </Suspense>
    ),
  },
  {
    path: "/student/score",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <StudentScore />
      </Suspense>
    ),
  },
];

const privateRouter = [
  {
    element: (
      <ProtectedRoute requiredRoles={["Principal", "Teacher"]}>
        <DefaultLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        path: "/home",
        element: <div>Home</div>,
      },
      ...studentRouter,
      ...adminRouter,
      ...teacherRouter,
    ],
  },
];

export default AppRouter;
