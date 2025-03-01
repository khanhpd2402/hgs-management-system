import DefaultLayout from "@/layouts/DefaultLayout/DefaultLayout";
import { lazy, Suspense } from "react";
import { createBrowserRouter, RouterProvider } from "react-router";

const EmployeeTable = lazy(
  () => import("@/pages/Employee/Profile/EmployeeTable"),
);
const StudentTable = lazy(() => import("@/pages/Student/Profile/StudentTable"));
const TATable = lazy(
  () => import("@/pages/Employee/TeachingAssignment/TATable"),
);
const HTATable = lazy(
  () => import("@/pages/Employee/HeadTeacherAssignment/HTATable"),
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
        path: "/employee/profile",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <EmployeeTable />
          </Suspense>
        ),
      },
      {
        path: "/employee/teaching-assignment",
        element: (
          <Suspense fallback={<div>Loading...</div>}>
            <TATable />
          </Suspense>
        ),
      },
      {
        path: "/employee/head-teacher-assignment",
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
