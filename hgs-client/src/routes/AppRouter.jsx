import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import AuthLayout from "@/layouts/AuthLayout/AuthLayout";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import Login from "@/pages/Login/Login";
import UserManagement from "@/pages/Principal/UserManagement";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import GradeBatch from "@/pages/Teacher/GradeBatch/GradeBatch";
import MarkReportTable from "@/pages/Teacher/MarkReport/MarkReportTable";
import AddTeacher from "@/pages/Teacher/Profile/AddTeacher";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, RouterProvider } from "react-router";

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
  const routes = privateRouter;
  // const routes = authRoutes;

  const router = createBrowserRouter([...routes]);

  return <RouterProvider router={router} />;
};

const authRoutes = [
  {
    element: <AuthLayout />,
    children: [
      {
        path: "/login",
        element: <Login />,
      },
    ],
  },
];

const privateRouter = [
  {
    element: <DefaultLayout />,
    children: [...studentRouter, ...adminRouter, ...teacherRouter],
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
      <ErrorBoundary fallback={<FallbackErrorBoundary />}>
        <Suspense fallback={<div>Loading...</div>}>
          <TeacherTable />
        </Suspense>
      </ErrorBoundary>
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
      <Suspense fallback={<div>Loading...</div>}>
        <MarkReportTable />
      </Suspense>
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
    path: "/",
    element: <div>Home</div>,
  },

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

export default AppRouter;
