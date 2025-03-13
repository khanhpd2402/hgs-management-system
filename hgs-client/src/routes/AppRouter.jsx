import ErrorRouteComponent from "@/components/ErrorRouteComponent";
import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import MarkReportTable from "@/pages/Teacher/MarkReport/MarkReportTable";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, RouterProvider } from "react-router";

const TeacherTable = lazy(() => import("@/pages/Teacher/Profile/TeacherTable"));
const StudentTable = lazy(() => import("@/pages/Student/Profile/StudentTable"));
const TeacherProfile = lazy(
  () => import("@/pages/Teacher/Profile/TeacherProfile"),
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

  const router = createBrowserRouter([...routes]);

  return <RouterProvider router={router} />;
};

const privateRouter = [
  {
    element: <DefaultLayout />,

    children: [
      {
        path: "/",
        element: <div>Home</div>,
      },
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
        path: "/student/profile",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <StudentTable />
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
    ],
  },
];

export default AppRouter;
