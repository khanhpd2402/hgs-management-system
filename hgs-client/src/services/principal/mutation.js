import { useMutation } from "@tanstack/react-query";
import {
  addGradeBatch,
  changeUserStatus,
  configueSubject,
  createSubject,
  deleteSubjectConfigue,
  resetUserPassword,
  updateSubject,
  updateSubjectConfigue,
} from "./api";
import toast from "react-hot-toast";
import { useQueryClient } from "@tanstack/react-query";

export function useAddGradeBatch() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data) => {
      return addGradeBatch(data);
    },
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        toast.error("Thêm thất bại");
      } else {
        console.log(data);
        queryClient.invalidateQueries({ queryKey: ["grade-batchs"] });
        toast.success("Thêm thành công");
      }
    },
  });
}

export function useChangeStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ userId, status }) => {
      return changeUserStatus(userId, status);
    },
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        toast.error("Thay đổi trạng thái thất bại");
      } else {
        console.log(data);
        queryClient.invalidateQueries({ queryKey: ["users"] });
        // toast.success("Thay đổi trạng thái thành công");
      }
    },
  });
}

export function useResetPassword() {
  return useMutation({
    mutationFn: ({ userId, newPassword }) => {
      return resetUserPassword(userId, newPassword);
    },
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        toast.error("Đặt lại mật khẩu thất bại");
      } else {
        console.log(data);
        toast.success("Đặt lại mật khẩu thành công");
      }
    },
  });
}

// export function useConfigueSubject() {
//   const queryClient = useQueryClient();
//   return useMutation({
//     mutationFn: (data) => {
//       data.map((item) => {
//         return configueSubject(item);
//       });
//     },
//     onSettled: (data, error) => {
//       if (error) {
//         console.log(error);
//         toast.error("Cấu hình môn học thất bại");
//       } else {
//         console.log(data);
//         queryClient.invalidateQueries({ queryKey: ["subjects"] });
//         toast.success("Cấu hình môn học thành công");
//       }
//     },
//   });
// }

export function useCreateSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data) => {
      const subjectData = {
        subjectName: data.subjectName,
        typeOfGrade: data.subjectCode,
        subjectCategory: data.subjectCategory,
      };
      return createSubject(data);
    },
    onSettled: async (data, error, variables) => {
      if (error) {
        console.log(error);
        toast.error("Tạo môn học thất bại");
      } else {
        console.log(data);
        const newData = variables?.gradesData.map((item) => {
          return {
            subjectId: data.subjectId,
            ...item,
          };
        });
        for (let i = 0; i < newData.length; i++) {
          try {
            console.log(newData[i]);
            await configueSubject(newData[i]);
          } catch (error) {
            console.log(error);
            toast.error("Cấu hình môn học thất bại");
          }
        }
        queryClient.invalidateQueries({ queryKey: ["subjects"] });
        toast.success("Tạo môn học thành công");
      }
    },
  });
}

export function useUpdateSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data) => {
      const subjectData = {
        subjectName: data.subjectName,
        typeOfGrade: data.typeOfGrade,
        subjectCategory: data.subjectCategory,
      };
      return updateSubject(data.subjectId, subjectData);
    },
    onSettled: async (data, error, variables) => {
      if (error) {
        console.log(error);
        toast.error("Cập nhật môn học thất bại");
      } else {
        const configData = variables?.gradesData.map((item) => {
          return {
            subjectId: data.subjectId,
            ...item,
          };
        });
        for (let i = 0; i < configData.length; i++) {
          try {
            if (configData[i].status === "add") {
              const { status, ...newData } = configData[i];
              await configueSubject(newData);
            } else if (configData[i].status === "update") {
              const { status, ...newData } = configData[i];
              await updateSubjectConfigue(newData.gradeLevelSubjectId, newData);
            } else if (configData[i].status === "delete") {
              const { status, ...newData } = configData[i];
              await deleteSubjectConfigue(newData.gradeLevelSubjectId);
            }
          } catch (e) {
            console.log(e);
            toast.error("Cấu hình môn học thất bại");
          }
        }
        queryClient.invalidateQueries({ queryKey: ["subjects"] });
        queryClient.invalidateQueries({
          queryKey: ["subjectConfig", { id: variables.subjectId }],
        });
        toast.success("Cập nhật môn học thành công");
      }
    },
  });
}
