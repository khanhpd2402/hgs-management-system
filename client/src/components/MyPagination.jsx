import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import PropTypes from "prop-types";

const MyPagination = ({ totalPages, currentPage, onPageChange }) => {
  const handlePageChange = (page) => {
    if (page < 1 || page > totalPages) return;
    onPageChange(page);
  };

  const pageNumbers = [];
  if (totalPages <= 7) {
    // Hiển thị toàn bộ nếu totalPages nhỏ hơn hoặc bằng 7
    for (let i = 1; i <= totalPages; i++) {
      pageNumbers.push(i);
    }
  } else {
    // Thêm trang đầu tiên nếu currentPage > 3
    if (currentPage > 3) {
      pageNumbers.push(1);
    }

    // Thêm dấu ... nếu currentPage > 4
    if (currentPage > 4) {
      pageNumbers.push("ellipsis-start");
    }

    // Hiển thị các trang gần currentPage
    for (
      let i = Math.max(1, currentPage - 2);
      i <= Math.min(totalPages, currentPage + 2);
      i++
    ) {
      pageNumbers.push(i);
    }

    // Thêm dấu ... nếu currentPage < totalPages - 3
    if (currentPage < totalPages - 3) {
      pageNumbers.push("ellipsis-end");
    }

    // Thêm trang cuối cùng nếu currentPage < totalPages - 2
    if (currentPage < totalPages - 2) {
      pageNumbers.push(totalPages);
    }
  }

  return (
    <Pagination>
      <PaginationContent>
        {/* Nút Previous */}
        <PaginationItem>
          <PaginationPrevious
            href="#"
            onClick={() => handlePageChange(currentPage - 1)}
            disabled={currentPage === 1}
            style={{ backgroundColor: currentPage === 1 && "#E5E7EB" }}
          />
        </PaginationItem>

        {/* Hiển thị danh sách trang */}
        {pageNumbers.map((page, index) =>
          typeof page === "number" ? (
            <PaginationItem key={index}>
              <PaginationLink
                href="#"
                isActive={page === currentPage}
                onClick={() => handlePageChange(page)}
              >
                {page}
              </PaginationLink>
            </PaginationItem>
          ) : (
            <PaginationItem key={index}>
              <PaginationEllipsis />
            </PaginationItem>
          ),
        )}

        {/* Nút Next */}
        <PaginationItem>
          <PaginationNext
            href="#"
            onClick={() => handlePageChange(currentPage + 1)}
            disabled={currentPage === totalPages}
            style={{ backgroundColor: currentPage === totalPages && "#E5E7EB" }}
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
};

MyPagination.propTypes = {
  totalPages: PropTypes.number.isRequired,
  currentPage: PropTypes.number.isRequired,
  onPageChange: PropTypes.func.isRequired,
};

export default MyPagination;
