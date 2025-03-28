import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import AuthLayout from "@/layouts/AuthLayout/AuthLayout";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import Login from "@/pages/Login/Login";
import UserManagement from "@/pages/Principal/UserProfile/UserManagement";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import GradeBatch from "@/pages/Teacher/GradeBatch/GradeBatch";
import MarkReportTable from "@/pages/Teacher/MarkReport/MarkReportTable";
import AddTeacher from "@/pages/Teacher/Profile/AddTeacher";
import ProtectedRoute from "@/routes/ProtectedRoute";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, Navigate, RouterProvider } from "react-router";
import AuthRedirectRoute from "./AuthRedirectRoute";
import ErrorRouteComponent from "@/components/ErrorRouteComponent";
import ScheduleTable from "@/pages/Schedule/ScheduleSymtem/Schedule";
import ScheduleTeacher from "@/pages/Schedule/ScheduleTeacher/ScheduleTeacher";
import RequestLessonPlan from "@/pages/RequestLessonPlan/RequestLessonPlan";
import UploadLessonPlan from "@/pages/RequestLessonPlan/UploadLessonPlan";

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
  const routes = [...privateRouter, ...authRoutes, ...systemRouter];
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
    path: "/principal/user",
    element: (
      <ProtectedRoute requiredRoles={["Principal"]}>
        <UserManagement />
      </ProtectedRoute>
    ),
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
  {
    path: "/teacher/schedule",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <ScheduleTeacher />
      </Suspense>
    ),
  },
  {
    path: "/teacher/request-lesson-plan",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <RequestLessonPlan />
      </Suspense>
    ),
  },
  {
    path: "/teacher/upload-lesson-plan",
    element: (
      <Suspense fallback={<div>Loading...</div>}>
        <UploadLessonPlan />
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
      {
        path: "/",
        element: <Navigate to="/home" />,
      },
      {
        path: "*",
        element: <ErrorRouteComponent />,
      },
      ...studentRouter,
      ...adminRouter,
      ...teacherRouter,
    ],
  },
];

const systemRouter = [
  {
    element: (
      <DefaultLayout />
    ),
    children: [
      {
        path: "/system/schedule",
        element: <ScheduleTable />,
      },
    ],
  },
];

export default AppRouter;
