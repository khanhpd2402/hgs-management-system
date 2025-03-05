import ErrorRouteComponent from "@/components/ErrorRouteComponent";
import { FallbackErrorBoundary } from "@/components/FallbackErrorBoundary";
import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import AttendanceTable from "@/pages/Teacher/Attendance/AttendanceTable";
import { lazy, Suspense } from "react";
import { ErrorBoundary } from "react-error-boundary";
import { createBrowserRouter, RouterProvider } from "react-router";

const TeacherTable = lazy(() => import("@/pages/Teacher/Profile/TeacherTable"));
const StudentTable = lazy(() => import("@/pages/Student/Profile/StudentTable"));
const TATable = lazy(
  () => import("@/pages/Teacher/TeachingAssignment/TATable"),
);
const HTATable = lazy(
  () => import("@/pages/Teacher/HeadTeacherAssignment/HTATable"),
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
        path: "/student/profile",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <StudentTable />
          </Suspense>
        ),
      },
    ],
  },
];

export default AppRouter;
