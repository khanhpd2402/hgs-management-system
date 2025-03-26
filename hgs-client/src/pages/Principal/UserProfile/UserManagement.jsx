import { useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent } from "@/components/ui/card";
import MyPagination from "@/components/MyPagination";
import PaginationControls from "@/components/PaginationControls";
import { Key, Search, Lock, UserRound } from "lucide-react";
import { useUsers } from "@/services/principal/queries";
import { useChangeStatus } from "@/services/principal/mutation";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import CustomTooltip from "@/components/common/CustomToolTip";
import toast from "react-hot-toast";

const UserManagement = () => {
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 10,
    search: "",
  });
  const [statusDialog, setStatusDialog] = useState({
    isOpen: false,
    userId: null,
    currentStatus: null,
  });

  const statusMutation = useChangeStatus();
  const userQuery = useUsers();
  const allUsers = userQuery.data || [];
  const { page, pageSize, search } = filter;

  // Handle status change confirmation
  const handleStatusChange = () => {
    if (!statusDialog.userId) return;

    const newStatus =
      statusDialog.currentStatus === "Active" ? "Deactive" : "Active";

    statusMutation.mutate(
      {
        userId: statusDialog.userId,
        status: newStatus,
      },
      {
        onSuccess: () => {
          toast.success(
            `Đã ${newStatus === "Active" ? "mở khóa" : "khóa"} tài khoản thành công`,
          );
          closeStatusDialog();
        },
        onError: (error) => {
          toast.error(
            `Không thể thay đổi trạng thái tài khoản: ${error.message}`,
          );
        },
      },
    );
  };

  const openStatusDialog = (userId, currentStatus) => {
    setStatusDialog({
      isOpen: true,
      userId,
      currentStatus,
    });
  };

  const closeStatusDialog = () => {
    setStatusDialog({
      isOpen: false,
      userId: null,
      currentStatus: null,
    });
  };

  const filteredData =
    allUsers.filter((teacher) => {
      if (search) {
        const searchLower = search.toLowerCase();
        return (
          teacher.fullName?.toLowerCase().includes(searchLower) ||
          teacher.email?.toLowerCase().includes(searchLower) ||
          teacher.phoneNumber?.toLowerCase().includes(searchLower)
        );
      }

      return true;
    }) || [];

  const totalPages = Math.ceil(filteredData.length / pageSize);
  const currentData = filteredData.slice(
    (page - 1) * pageSize,
    page * pageSize,
  );

  const startIndex = filteredData.length === 0 ? 0 : (page - 1) * pageSize + 1;
  const endIndex = Math.min(page * pageSize, filteredData.length);

  return (
    <div className="py-6">
      <h1 className="mb-6 text-2xl font-bold">Quản lý người dùng</h1>

      <div className="mb-6 flex items-center justify-between">
        <div className="relative w-64">
          <Input
            placeholder="Tìm kiếm người dùng..."
            value={filter.search}
            onChange={(e) =>
              setFilter({ ...filter, search: e.target.value, page: 1 })
            }
            className="pl-10"
          />
          <Search className="absolute top-2.5 left-3 h-4 w-4 text-gray-400" />
        </div>
        <Button className="bg-blue-600 hover:bg-blue-700">
          Thêm người dùng
        </Button>
      </div>

      <Card className="mb-6 overflow-hidden border border-gray-200">
        <CardContent className="p-0">
          <div className="max-h-[500px] overflow-auto">
            <Table className="table-fixed border-collapse">
              <TableHeader className="bg-slate-100">
                <TableRow className="border-b border-gray-200">
                  <TableHead className="w-24 border border-gray-200 text-center">
                    Thao tác
                  </TableHead>
                  <TableHead className="w-16 border border-gray-200 text-center">
                    STT
                  </TableHead>
                  <TableHead className="w-48 border border-gray-200">
                    Tên cán bộ
                  </TableHead>
                  <TableHead className="w-44 border border-gray-200">
                    Tên đăng nhập
                  </TableHead>
                  <TableHead className="w-64 border border-gray-200">
                    Email
                  </TableHead>
                  <TableHead className="w-32 border border-gray-200">
                    Điện thoại
                  </TableHead>

                  <TableHead className="w-32 border border-gray-200 text-center">
                    Trạng thái
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {userQuery.isPending ? (
                  <TableRow>
                    <TableCell
                      colSpan={8}
                      className="border border-gray-200 text-center"
                    >
                      Đang tải dữ liệu...
                    </TableCell>
                  </TableRow>
                ) : currentData.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={10}
                      className="border border-gray-200 text-center"
                    >
                      Không tìm thấy người dùng nào
                    </TableCell>
                  </TableRow>
                ) : (
                  currentData.map((user, index) => (
                    <TableRow
                      key={user.userId}
                      className="border-b border-gray-200"
                    >
                      <TableCell className="border border-gray-200">
                        <div className="flex justify-center space-x-2">
                          <CustomTooltip content="Thay đổi quyền">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8 cursor-pointer rounded-full bg-gray-100"
                            >
                              <UserRound className="h-4 w-4 text-amber-500" />
                            </Button>
                          </CustomTooltip>

                          <CustomTooltip content="Thay đổi khoá">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8 cursor-pointer rounded-full bg-gray-100"
                              onClick={() =>
                                openStatusDialog(user.userId, user.status)
                              }
                            >
                              <Lock className="h-4 w-4 text-blue-500" />
                            </Button>
                          </CustomTooltip>

                          <CustomTooltip content="Đặt lại mật khẩu">
                            <Button
                              variant="ghost"
                              size="icon"
                              className="h-8 w-8 cursor-pointer rounded-full bg-gray-100"
                            >
                              <Key className="h-4 w-4 text-green-500" />
                            </Button>
                          </CustomTooltip>
                        </div>
                      </TableCell>
                      <TableCell className="border border-gray-200 text-center">
                        {startIndex + index}
                      </TableCell>
                      <TableCell className="border border-gray-200">
                        {user.fullName}
                      </TableCell>
                      <TableCell className="border border-gray-200">
                        {user.username}
                      </TableCell>
                      <TableCell className="border border-gray-200">
                        {user.email}
                      </TableCell>
                      <TableCell className="border border-gray-200">
                        {user.phone}
                      </TableCell>
                      <TableCell className="border border-gray-200 text-center">
                        <Badge
                          className={
                            user.status === "Active"
                              ? "bg-emerald-500 hover:bg-emerald-600"
                              : "bg-red-500 hover:bg-red-600"
                          }
                          variant="default"
                        >
                          {user.status === "Active"
                            ? "Hoạt động"
                            : "Không hoạt động"}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      <div className="flex items-center justify-between">
        <PaginationControls
          pageSize={pageSize}
          setFilter={setFilter}
          totalItems={filteredData.length || 0}
          startIndex={startIndex}
          endIndex={endIndex}
        />

        <MyPagination
          totalPages={totalPages}
          currentPage={page}
          onPageChange={setFilter}
        />
      </div>

      {/* Status Change Confirmation Dialog */}
      <Dialog open={statusDialog.isOpen} onOpenChange={closeStatusDialog}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>
              {statusDialog.currentStatus === "Active"
                ? "Khóa tài khoản"
                : "Mở khóa tài khoản"}
            </DialogTitle>
            <DialogDescription>
              {statusDialog.currentStatus === "Active"
                ? "Bạn có chắc chắn muốn khóa tài khoản này? Người dùng sẽ không thể đăng nhập vào hệ thống."
                : "Bạn có chắc chắn muốn mở khóa tài khoản này? Người dùng sẽ có thể đăng nhập vào hệ thống."}
            </DialogDescription>
          </DialogHeader>
          <DialogFooter className="sm:justify-end">
            <Button
              type="button"
              variant="secondary"
              onClick={closeStatusDialog}
              className="mt-2 sm:mt-0"
            >
              Hủy
            </Button>
            <Button
              type="button"
              variant={
                statusDialog.currentStatus === "Active"
                  ? "destructive"
                  : "default"
              }
              onClick={handleStatusChange}
              className="mt-2 sm:mt-0"
              disabled={statusMutation.isPending}
            >
              {statusMutation.isPending
                ? "Đang xử lý..."
                : statusDialog.currentStatus === "Active"
                  ? "Khóa tài khoản"
                  : "Mở khóa tài khoản"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default UserManagement;
