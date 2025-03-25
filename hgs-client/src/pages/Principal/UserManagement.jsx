import { useState, useEffect } from "react";
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
import { Checkbox } from "@/components/ui/checkbox";
import { Card, CardContent } from "@/components/ui/card";
import MyPagination from "@/components/MyPagination";
import PaginationControls from "@/components/PaginationControls";
import { Edit, Key, UserRound, Search } from "lucide-react";

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedUsers, setSelectedUsers] = useState([]);
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 10,
    search: "",
  });
  const [totalItems, setTotalItems] = useState(0);

  // Mock data for demonstration
  useEffect(() => {
    // In a real application, this would be an API call
    const mockUsers = [
      {
        id: 1,
        name: "Lê Thị Hằng",
        username: "thcshg_lehang",
        email: "Hangphuongbch@gmail.com",
        phone: "0917824977",
        lastLogin: "12/09/2024 02:55:52",
        userGroup: "239127",
        status: "active",
      },
      {
        id: 2,
        name: "Vương Thị Ngọc Anh",
        username: "thcshg_ngoccanh",
        email: "anhngoc24062000@gmail.com",
        phone: "0373405863",
        lastLogin: "27/02/2025 08:27:40",
        userGroup: "239116",
        status: "active",
      },
      {
        id: 3,
        name: "Phạm Thị Thuỷ",
        username: "thcshg_phamthithuy",
        email: "thuy1980pham@gmail.com",
        phone: "0964670817",
        lastLogin: "23/03/2025 08:06:06",
        userGroup: "239116",
        status: "active",
      },
      {
        id: 4,
        name: "Đỗ Thị Thu",
        username: "thcshg_dothithu",
        email: "dothu8983@gmail.com",
        phone: "0989762305",
        lastLogin: "23/03/2025 03:20:16",
        userGroup: "239117, 239116",
        status: "active",
      },
      {
        id: 5,
        name: "Vũ Thị Thu Hoài",
        username: "thcshg_vuthithuhoai",
        email: "vuhoaihaigiang@gmail.com",
        phone: "0947945228",
        lastLogin: "23/03/2025 04:36:15",
        userGroup: "239117, 239116",
        status: "active",
      },
      {
        id: 6,
        name: "Nguyễn Thanh Trung",
        username: "thcshg_nguyenthanhtrung",
        email: "trungvoc@gmail.com",
        phone: "0912731512",
        lastLogin: "25/03/2025 01:46:05",
        userGroup: "239116",
        status: "active",
      },
      {
        id: 7,
        name: "Nguyễn Ngọc Trang",
        username: "thcshg_nguyenngoctrang",
        email: "ntrang79@gmail.com",
        phone: "0977442719",
        lastLogin: "22/03/2025 02:28:43",
        userGroup: "239117, 239116",
        status: "active",
      },
      {
        id: 8,
        name: "Trần Thị Minh Nguyệt",
        username: "thcshg_tranthiminhnguyet",
        email: "nguyetthcshaigiang@gmail.com",
        phone: "0357539288",
        lastLogin: "14/02/2025 08:24:05",
        userGroup: "239116",
        status: "active",
      },
      {
        id: 9,
        name: "Trần Thị Lan",
        username: "thcshg_tranthilan",
        email: "lamtranngoccanh2017@gmail.com",
        phone: "0974559178",
        lastLogin: "18/09/2021 09:01:15",
        userGroup: "239127",
        status: "active",
      },
      {
        id: 10,
        name: "Nguyễn Thị Huyền",
        username: "thcshg_nguyenthihuyen",
        email: "nguyenthihuyenhaian@gmail.com",
        phone: "0988886828",
        lastLogin: "03/03/2025 10:08:11",
        userGroup: "239117, 239116",
        status: "active",
      },
    ];

    // Filter users based on search term
    const filteredUsers = mockUsers.filter(
      (user) =>
        user.name.toLowerCase().includes(filter.search.toLowerCase()) ||
        user.username.toLowerCase().includes(filter.search.toLowerCase()) ||
        user.email.toLowerCase().includes(filter.search.toLowerCase()),
    );

    setTotalItems(filteredUsers.length);

    // Paginate the results
    const startIndex = (filter.page - 1) * filter.pageSize;
    const paginatedUsers = filteredUsers.slice(
      startIndex,
      startIndex + filter.pageSize,
    );

    setUsers(paginatedUsers);
    setLoading(false);
  }, [filter]);

  const totalPages = Math.ceil(totalItems / filter.pageSize);
  const startIndex = (filter.page - 1) * filter.pageSize + 1;
  const endIndex = Math.min(startIndex + filter.pageSize - 1, totalItems);

  return (
    <div className="container mx-auto py-6">
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
          <div className="overflow-x-auto">
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
                {loading ? (
                  <TableRow>
                    <TableCell
                      colSpan={8}
                      className="border border-gray-200 text-center"
                    >
                      Đang tải dữ liệu...
                    </TableCell>
                  </TableRow>
                ) : users.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={10}
                      className="border border-gray-200 text-center"
                    >
                      Không tìm thấy người dùng nào
                    </TableCell>
                  </TableRow>
                ) : (
                  users.map((user, index) => (
                    <TableRow
                      key={user.id}
                      className="border-b border-gray-200"
                    >
                      <TableCell className="border border-gray-200">
                        <div className="flex justify-center space-x-2">
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 rounded-full bg-gray-100"
                          >
                            <Edit className="h-4 w-4 text-amber-500" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 rounded-full bg-gray-100"
                          >
                            <UserRound className="h-4 w-4 text-blue-500" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="icon"
                            className="h-8 w-8 rounded-full bg-gray-100"
                          >
                            <Key className="h-4 w-4 text-green-500" />
                          </Button>
                        </div>
                      </TableCell>
                      <TableCell className="border border-gray-200 text-center">
                        {startIndex + index}
                      </TableCell>
                      <TableCell className="border border-gray-200">
                        {user.name}
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
                          className="bg-emerald-500 hover:bg-emerald-600"
                          variant="default"
                        >
                          Hoạt động
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
          pageSize={filter.pageSize}
          setFilter={setFilter}
          totalItems={totalItems}
          startIndex={startIndex}
          endIndex={endIndex}
        />
        <MyPagination
          totalPages={totalPages}
          currentPage={filter.page}
          onPageChange={setFilter}
        />
      </div>
    </div>
  );
};

export default UserManagement;
