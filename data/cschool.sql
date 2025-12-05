-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 127.0.0.1
-- Thời gian đã tạo: Th11 28, 2025 lúc 10:00 AM  (Cập nhật ngày hiện tại)
-- Phiên bản máy phục vụ: 10.4.32-MariaDB
-- Phiên bản PHP: 8.1.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `cschool`
--
DROP DATABASE IF EXISTS cschool;
CREATE DATABASE cschool;
USE cschool;
-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `assign_classes`
--

CREATE TABLE `assign_classes` (
  `id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `head_teacher_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `assign_classes`
--

INSERT INTO `assign_classes` (`id`, `class_id`, `head_teacher_id`, `term_id`, `status`) VALUES
(11, 1, 1, 1, 1),
(12, 2, 2, 1, 1),
(13, 3, 3, 6, 1),
(14, 4, 4, 3, 1),
(15, 5, 5, 4, 1),
(16, 6, 6, 2, 1),
(17, 7, 7, 5, 1),
(18, 8, 8, 6, 1),
(19, 2, 9, 7, 1),
(20, 1, 10, 8, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `assign_class_students`
--

CREATE TABLE `assign_class_students` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `role` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `assign_class_students`
--

INSERT INTO `assign_class_students` (`assign_class_id`, `student_id`, `role`) VALUES
(11, 1, 'Lớp trưởng'),
(11, 21, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(12, 2, 'Lớp phó học tập'),
(12, 3, 'Thành viên'),  -- Thêm để nhất quán với score_details
(12, 4, 'Thành viên'),  -- Thêm để nhất quán với score_details
(12, 5, 'Thành viên'),  -- Thêm để nhất quán với score_details
(12, 6, 'Thành viên'),  -- Thêm để nhất quán với score_details
(12, 7, 'Thành viên'),  -- Thêm để nhất quán với score_details
(13, 3, 'Lớp phó lao động'),
(13, 5, 'Thành viên'),
(13, 6, 'Thành viên'),
(13, 23, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(14, 4, 'Thành viên'),
(14, 7, 'Thành viên'),
(14, 24, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(15, 9, 'Thành viên'),
(15, 25, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(16, 8, 'Thành viên'),
(16, 26, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(17, 10, 'Thành viên'),
(17, 27, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(18, 28, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(19, 29, 'Thành viên'),  -- Thêm để nhất quán với subject_term_avg và term_gpa
(20, 30, 'Thành viên');  -- Thêm để nhất quán với subject_term_avg và term_gpa

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `assign_class_teachers`
--

CREATE TABLE `assign_class_teachers` (
  `assign_class_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `quiz_count` int(11) DEFAULT NULL,
  `oral_count` int(11) DEFAULT NULL,
  `day` varchar(100) NOT NULL,
  `start_period` int(11) DEFAULT NULL,
  `end_period` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `assign_class_teachers`
--

INSERT INTO `assign_class_teachers` (`assign_class_id`, `teacher_id`, `subject_id`, `quiz_count`, `oral_count`, `day`, `start_period`, `end_period`) VALUES
(11, 2, 1, 2, 2, 'Thứ Ba', 2, 5),
(11, 3, 7, 2, 2, 'Thứ Bảy', 1, 5),
(11, 1, 9, 1, 1, 'Thứ Hai', 1, 2),
(12, 1, 1, 2, 2, 'Thứ Hai', 1, 3),  -- Thêm để nhất quán với score_details (Toán học)
(12, 2, 2, 2, 2, 'Thứ Tư', 2, 4),  -- Thêm để nhất quán với score_details (Ngữ văn)
(13, 1, 4, 1, 2, 'Thứ Ba', 2, 4),
(13, 3, 5, 1, 1, 'Thứ Ba', 2, 3),
(14, 4, 1, 2, 2, 'Thứ Năm', 4, 5),
(15, 5, 6, 3, 1, 'Thứ Ba', 1, 2),
(16, 6, 9, 2, 2, 'Thứ Tư', 3, 4),
(19, 2, 1, 1, 2, 'Thứ Hai', 2, 4),
(19, 9, 3, 1, 1, 'Thứ Năm', 1, 2),
(20, 10, 4, 2, 2, 'Thứ Ba', 3, 4);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `classes`
--

CREATE TABLE `classes` (
  `id` int(11) NOT NULL,
  `class_type_id` int(11) NOT NULL,
  `grade` int(11) DEFAULT NULL,
  `name` varchar(100) NOT NULL,
  `area` varchar(50) DEFAULT NULL,
  `room` varchar(50) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `classes`
--

INSERT INTO `classes` (`id`, `class_type_id`, `grade`, `name`, `area`, `room`, `status`) VALUES
(1, 2, 10, '10A1', 'Khu A', 'Phòng 101', 1),
(2, 2, 10, '10A3', 'Khu A', 'Phòng 102', 1),
(3, 2, 10, '10A2', 'Khu B', 'Phòng 201', 1),
(4, 3, 11, '11A1', 'Khu B', 'Phòng 202', 1),
(5, 3, 11, '11A2', 'Khu C', 'Phòng 301', 1),
(6, 4, 12, '12A1', 'Khu C', 'Phòng 302', 1),
(7, 4, 12, '12A3', 'Khu D', 'Phòng 401', 1),
(8, 7, 12, '12A2', 'Khu D', 'Phòng 405', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `class_types`
--

CREATE TABLE `class_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `class_types`
--

INSERT INTO `class_types` (`id`, `name`, `description`, `status`) VALUES
(1, 'Chuyên anh', '', 1),
(2, 'Chuyên toán', '', 1),
(3, 'Chuyên văn', '', 1),
(4, 'Chuyên Lí', '', 1),
(5, 'Chuyên tin', '', 1),
(6, 'Chuyên hóa', '', 1),
(7, 'Không chuyên', '', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `departments`
--

CREATE TABLE `departments` (
  `id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `departments`
--

INSERT INTO `departments` (`id`, `subject_id`, `name`, `description`, `status`) VALUES
(1, 1, 'Tổ Toán', 'Phụ trách giảng dạy và phát triển chương trình môn Toán học.', 1),
(2, 2, 'Tổ Ngữ Văn', 'Chuyên môn về giảng dạy tiếng Việt và văn học.', 1),
(3, 3, 'Tổ Tiếng Anh', 'Phụ trách đào tạo và kiểm tra năng lực tiếng Anh học sinh.', 1),
(4, 4, 'Tổ Vật Lý', 'Tổ chuyên môn nghiên cứu và giảng dạy các kiến thức vật lý.', 1),
(5, 5, 'Tổ Hóa Học', 'Phụ trách giảng dạy môn Hóa và tổ chức thí nghiệm thực hành.', 1),
(6, 6, 'Tổ Sinh Học', 'Chuyên môn về sinh học, môi trường và sức khỏe học đường.', 1),
(7, 7, 'Tổ Lịch Sử', 'Tổ chức các hoạt động học tập và ngoại khóa về lịch sử dân tộc.', 1),
(8, 8, 'Tổ Địa Lý', 'Phụ trách giảng dạy kiến thức địa lý và kỹ năng bản đồ.', 1),
(9, 9, 'Tổ Tin Học', 'Chuyên môn về công nghệ thông tin, lập trình và kỹ năng số.', 1),
(10, 10, 'Tổ GDCD', 'Giảng dạy đạo đức, pháp luật và kỹ năng sống cho học sinh.', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `department_details`
--

CREATE TABLE `department_details` (
  `department_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `role` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `department_details`
--

INSERT INTO `department_details` (`department_id`, `teacher_id`, `start_date`, `end_date`, `role`) VALUES
(1, 1, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Toán'),
(2, 2, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Ngữ Văn'),
(3, 3, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Tiếng Anh'),
(4, 4, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Vật Lý'),
(5, 5, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Hóa Học'),
(6, 6, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Sinh Học'),
(7, 7, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Lịch Sử'),
(8, 8, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Địa Lý'),
(9, 9, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ Tin Học'),
(10, 10, '2022-09-01', '2025-06-30', 'Tổ trưởng tổ GDCD');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `exams`
--

CREATE TABLE `exams` (
  `id` int(11) NOT NULL,
  `exam_detail_id` int(11) NOT NULL,
  `exam_room` varchar(50) DEFAULT NULL,
  `candidate_count` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `exams`
--

INSERT INTO `exams` (`id`, `exam_detail_id`, `exam_room`, `candidate_count`) VALUES
(1, 1, 'Phòng A101', 30),
(2, 2, 'Phòng A102', 28),
(3, 3, 'Phòng B201', 32),
(4, 4, 'Phòng B202', 29),
(5, 5, 'Phòng C301', 31),
(6, 6, 'Phòng C302', 27),
(7, 7, 'Phòng D401', 33),
(8, 8, 'Phòng D402', 30),
(9, 9, 'Phòng E501', 26),
(10, 10, 'Phòng E502', 34);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `exam_details`
--

CREATE TABLE `exam_details` (
  `id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `start_time` datetime DEFAULT NULL,
  `end_time` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `exam_details`
--

INSERT INTO `exam_details` (`id`, `subject_id`, `term_id`, `start_time`, `end_time`) VALUES
(1, 1, 1, '2025-11-01 08:00:00', '2025-11-01 09:30:00'),
(2, 2, 1, '2025-11-02 08:00:00', '2025-11-02 09:30:00'),
(3, 3, 1, '2025-11-03 08:00:00', '2025-11-03 09:30:00'),
(4, 4, 1, '2025-11-04 08:00:00', '2025-11-04 09:30:00'),
(5, 5, 1, '2025-11-05 08:00:00', '2025-11-05 09:30:00'),
(6, 6, 2, '2026-04-01 08:00:00', '2026-04-01 09:30:00'),
(7, 7, 2, '2026-04-02 08:00:00', '2026-04-02 09:30:00'),
(8, 8, 2, '2026-04-03 08:00:00', '2026-04-03 09:30:00'),
(9, 9, 2, '2026-04-04 08:00:00', '2026-04-04 09:30:00'),
(10, 10, 2, '2026-04-05 08:00:00', '2026-04-05 09:30:00');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `exam_types`
--

CREATE TABLE `exam_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `weight` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `exam_types`
--

INSERT INTO `exam_types` (`id`, `name`, `weight`) VALUES
(1, 'Kiểm tra miệng', 1),
(2, 'Kiểm tra 15 phút', 1),
(3, 'Thi giữa kỳ', 2),
(4, 'Thi cuối kỳ', 3);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `functions`
--

CREATE TABLE `functions` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `icon` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `functions`
--

INSERT INTO `functions` (`id`, `name`, `icon`) VALUES
(1, 'Thêm', 'fa-plus'),
(2, 'Sửa', 'fa-edit'),
(3, 'Xóa', 'fa-trash'),
(4, 'Phân lớp', 'fa-users'),
(5, 'Nhập điểm', 'fa-keyboard');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `relations`
--

CREATE TABLE `relations` (
  `id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `phone` varchar(10) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `job` varchar(100) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `relations`
--

INSERT INTO `relations` (`id`, `student_id`, `fullname`, `phone`, `birthday`, `email`, `job`, `status`) VALUES
(1, 1, 'Nguyễn Văn Bảo', '0901123456', '1975-04-12', 'nvb1@example.com', 'Kỹ sư', 1),
(2, 2, 'Trần Thị Mai', '0902234567', '1980-06-25', 'ttm2@example.com', 'Giáo viên', 1),
(3, 3, 'Lê Văn Hùng', '0903345678', '1972-09-18', 'lvh3@example.com', 'Bác sĩ', 1),
(4, 4, 'Phạm Thị Lan', '0904456789', '1983-11-03', 'ptl4@example.com', 'Kế toán', 1),
(5, 5, 'Hoàng Văn Minh', '0905567890', '1978-02-14', 'hvm5@example.com', 'Tài xế', 1),
(6, 6, 'Đặng Thị Hồng', '0906678901', '1985-08-30', 'dth6@example.com', 'Nhân viên văn phòng', 1),
(7, 7, 'Vũ Văn Quang', '0907789012', '1970-12-22', 'vvq7@example.com', 'Thợ điện', 1),
(8, 8, 'Bùi Thị Ngọc', '0908890123', '1982-03-09', 'btn8@example.com', 'Nội trợ', 1),
(9, 9, 'Ngô Văn Sơn', '0909901234', '1976-07-17', 'nvs9@example.com', 'Công nhân', 1),
(10, 10, 'Dương Thị Thu', '0910012345', '1978-07-17', 'dtt10@example.com', 'Luật sư', 1),
(11, 11, 'Nguyễn Văn Thành', '0911123456', '1974-01-01', 'nvt11@example.com', 'Kỹ sư', 1),
(12, 12, 'Trần Thị Hạnh', '0912234567', '1981-02-02', 'tth12@example.com', 'Giáo viên', 1),
(13, 13, 'Lê Văn Phúc', '0913345678', '1973-03-03', 'lvp13@example.com', 'Bác sĩ', 1),
(14, 14, 'Phạm Thị Hương', '0914456789', '1984-04-04', 'pth14@example.com', 'Kế toán', 1),
(15, 15, 'Hoàng Văn Tâm', '0915567890', '1979-05-05', 'hvt15@example.com', 'Tài xế', 1),
(16, 16, 'Đặng Thị Tuyết', '0916678901', '1986-06-06', 'dtt16@example.com', 'Nhân viên văn phòng', 1),
(17, 17, 'Vũ Văn Lâm', '0917789012', '1971-07-07', 'vvl17@example.com', 'Thợ điện', 1),
(18, 18, 'Bùi Thị Hoa', '0918890123', '1983-08-08', 'bth18@example.com', 'Nội trợ', 1),
(19, 19, 'Ngô Văn Dũng', '0919901234', '1977-09-09', 'nvd19@example.com', 'Công nhân', 1),
(20, 20, 'Dương Thị Yến', '0920012345', '1985-10-10', 'dty20@example.com', 'Luật sư', 1),
(21, 21, 'Nguyễn Văn Hòa', '0921123456', '1975-11-11', 'nvh21@example.com', 'Kỹ sư', 1),
(22, 22, 'Trần Thị Vân', '0922234567', '1980-12-12', 'ttv22@example.com', 'Giáo viên', 1),
(23, 23, 'Lê Văn Tài', '0923345678', '1972-01-13', 'lvt23@example.com', 'Bác sĩ', 1),
(24, 24, 'Phạm Thị Nga', '0924456789', '1983-02-14', 'ptn24@example.com', 'Kế toán', 1),
(25, 25, 'Hoàng Văn Phú', '0925567890', '1978-03-15', 'hvp25@example.com', 'Tài xế', 1),
(26, 26, 'Đặng Thị Kim', '0926678901', '1985-04-16', 'dtk26@example.com', 'Nhân viên văn phòng', 1),
(27, 27, 'Vũ Văn Đức', '0927789012', '1970-05-17', 'vvd27@example.com', 'Thợ điện', 1),
(28, 28, 'Bùi Thị Thảo', '0928890123', '1982-06-18', 'btt28@example.com', 'Nội trợ', 1),
(29, 29, 'Ngô Văn Khánh', '0929901234', '1976-07-19', 'nvk29@example.com', 'Công nhân', 1),
(30, 30, 'Dương Thị Hằng', '0930012345', '1984-08-20', 'dth30@example.com', 'Luật sư', 1),
(31, 31, 'Nguyễn Văn Lộc', '0931123456', '1975-09-21', 'nvl31@example.com', 'Kỹ sư', 1),
(32, 32, 'Trần Thị Xuân', '0932234567', '1980-10-22', 'ttx32@example.com', 'Giáo viên', 1),
(33, 33, 'Lê Văn Bình', '0933345678', '1972-11-23', 'lvb33@example.com', 'Bác sĩ', 1),
(34, 34, 'Phạm Thị Diễm', '0934456789', '1983-12-24', 'ptd34@example.com', 'Kế toán', 1),
(35, 35, 'Hoàng Văn Quý', '0935567890', '1978-01-25', 'hvq35@example.com', 'Tài xế', 1),
(36, 36, 'Đặng Thị Loan', '0936678901', '1985-02-26', 'dtl36@example.com', 'Nhân viên văn phòng', 1),
(37, 37, 'Vũ Văn Hưng', '0937789012', '1970-03-27', 'vvh37@example.com', 'Thợ điện', 1),
(38, 38, 'Bùi Thị Dung', '0938890123', '1982-04-28', 'btd38@example.com', 'Nội trợ', 1),
(39, 39, 'Ngô Văn Trí', '0939901234', '1976-05-29', 'nvt39@example.com', 'Công nhân', 1),
(40, 40, 'Dương Thị Nhung', '0940012345', '1984-06-30', 'dtn40@example.com', 'Luật sư', 1),
(41, 41, 'Nguyễn Văn Cường', '0941123456', '1975-07-01', 'nvc41@example.com', 'Kỹ sư', 1),
(42, 42, 'Trần Thị Huyền', '0942234567', '1980-08-02', 'tth42@example.com', 'Giáo viên', 1),
(43, 43, 'Lê Văn Sơn', '0943345678', '1972-09-03', 'lvs43@example.com', 'Bác sĩ', 1),
(44, 44, 'Phạm Thị Thanh', '0944456789', '1983-10-04', 'ptt44@example.com', 'Kế toán', 1),
(45, 45, 'Hoàng Văn Tùng', '0945567890', '1978-11-05', 'hvt45@example.com', 'Tài xế', 1),
(46, 46, 'Đặng Thị Hà', '0946678901', '1985-12-06', 'dth46@example.com', 'Nhân viên văn phòng', 1),
(47, 47, 'Vũ Văn Nam', '0947789012', '1970-01-07', 'vvn47@example.com', 'Thợ điện', 1),
(48, 48, 'Bùi Thị Tuyết', '0948890123', '1982-02-08', 'btt48@example.com', 'Nội trợ', 1),
(49, 49, 'Ngô Văn Lâm', '0949901234', '1976-03-09', 'nvl49@example.com', 'Công nhân', 1),
(50, 50, 'Dương Thị Hòa', '0950012345', '1984-04-10', 'dth50@example.com', 'Luật sư', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `roles`
--

CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `roles`
--

INSERT INTO `roles` (`id`, `name`, `status`) VALUES
(1, 'Quản trị viên', 1),
(2, 'Giáo viên', 1),
(3, 'Học sinh', 1),
(4, 'Phụ huynh', 1),
(5, 'Khách', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `role_details`
--

CREATE TABLE `role_details` (
  `role_id` int(11) NOT NULL,
  `function_id` int(11) NOT NULL,
  `action` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `role_details`
--

INSERT INTO `role_details` (`role_id`, `function_id`, `action`) VALUES
(1, 1, 'Thêm'),
(1, 2, 'Sửa'),
(1, 3, 'Xóa'),
(1, 4, 'Phân lớp'),
(1, 5, 'Nhập điểm'),
(2, 4, 'Phân lớp'),
(2, 5, 'Nhập điểm'),
(3, 5, 'Xem điểm'),
(4, 5, 'Xem điểm');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `rules`
--

CREATE TABLE `rules` (
  `id` int(11) NOT NULL,
  `title` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `rules`
--

INSERT INTO `rules` (`id`, `title`, `description`, `status`) VALUES
(1, 'Đi học đúng giờ', 'Học sinh phải có mặt tại lớp trước giờ học quy định.', 1),
(2, 'Mặc đồng phục', 'Học sinh phải mặc đồng phục theo quy định của nhà trường vào các ngày học.', 1),
(3, 'Giữ gìn vệ sinh', 'Không xả rác bừa bãi, giữ gìn vệ sinh lớp học và khuôn viên trường.', 1),
(4, 'Không sử dụng điện thoại', 'Không sử dụng điện thoại trong giờ học nếu không có sự cho phép của giáo viên.', 1),
(5, 'Tôn trọng thầy cô', 'Học sinh phải lễ phép, tôn trọng giáo viên và nhân viên nhà trường.', 1),
(6, 'Không gây mất trật tự', 'Không nói chuyện riêng, gây ồn ào trong lớp học và khu vực học tập.', 1),
(7, 'Tham gia đầy đủ các hoạt động', 'Học sinh phải tham gia đầy đủ các hoạt động ngoại khóa, sinh hoạt lớp và các buổi học chính khóa.', 1),
(8, 'Không hút thuốc', 'Nghiêm cấm học sinh hút thuốc trong khuôn viên trường học.', 1),
(9, 'Không đánh nhau', 'Cấm mọi hành vi gây gổ, đánh nhau trong và ngoài trường học.', 1),
(10, 'Bảo vệ tài sản chung', 'Không làm hư hỏng bàn ghế, thiết bị học tập và tài sản của nhà trường.', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `score_details`
--

CREATE TABLE `score_details` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `exam_type_id` int(11) NOT NULL,
  `attempt` int(11) NOT NULL,
  `score` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `score_details`
--

INSERT INTO `score_details` (`assign_class_id`, `student_id`, `subject_id`, `exam_type_id`, `attempt`, `score`) VALUES
(12, 2, 1, 1, 1, 7.00),
(12, 2, 1, 2, 1, 6.75),
(12, 2, 1, 3, 1, 9.00),
(12, 2, 1, 4, 1, 8.25),
(12, 2, 2, 1, 1, 9.50),
(12, 2, 2, 1, 2, 9.00),
(12, 2, 2, 1, 3, 8.50),
(12, 2, 2, 1, 4, 8.00),
(12, 3, 1, 1, 1, 8.50),
(12, 3, 1, 2, 1, 7.50),
(12, 3, 1, 3, 1, 7.50),
(12, 3, 1, 4, 1, 7.00),
(12, 3, 2, 1, 1, 9.50),
(12, 3, 2, 2, 1, 8.50),
(12, 3, 2, 3, 1, 7.50),
(12, 3, 2, 4, 1, 7.50),
(12, 4, 1, 1, 1, 8.50),
(12, 4, 1, 2, 1, 7.50),
(12, 4, 1, 3, 1, 7.75),
(12, 4, 1, 4, 1, 7.00),
(12, 4, 2, 1, 1, 9.75),
(12, 4, 2, 2, 1, 8.50),
(12, 4, 2, 3, 1, 7.00),
(12, 4, 2, 4, 1, 8.50),
(12, 5, 1, 1, 1, 8.50),
(12, 5, 1, 2, 1, 10.00),
(12, 5, 1, 3, 1, 7.50),
(12, 5, 1, 4, 1, 7.50),
(12, 5, 2, 1, 1, 9.50),
(12, 5, 2, 2, 1, 9.50),
(12, 5, 2, 3, 1, 7.00),
(12, 5, 2, 4, 1, 7.50),
(12, 6, 1, 1, 1, 8.50),
(12, 6, 1, 2, 1, 7.50),
(12, 6, 1, 3, 1, 9.50),
(12, 6, 1, 4, 1, 7.00),
(12, 6, 2, 1, 1, 9.50),
(12, 6, 2, 2, 1, 8.50),
(12, 6, 2, 3, 1, 7.50),
(12, 6, 2, 4, 1, 9.50),
(12, 7, 1, 1, 1, 10.00),
(12, 7, 1, 2, 1, 8.50),
(12, 7, 1, 3, 1, 7.50),
(12, 7, 1, 4, 1, 6.00),
(12, 7, 2, 1, 1, 5.50),
(12, 7, 2, 2, 1, 5.50),
(12, 7, 2, 3, 1, 7.50),
(12, 7, 2, 4, 1, 9.50);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `students`
--

CREATE TABLE `students` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `gender` enum('Male','Female','Other') DEFAULT NULL,
  `ethnicity` varchar(50) DEFAULT NULL,
  `religion` varchar(50) DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `phone` varchar(10) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `learn_year` varchar(100) DEFAULT NULL,
  `learn_status` varchar(15) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `students`
--

INSERT INTO `students` (`id`, `fullname`, `avatar`, `birthday`, `gender`, `ethnicity`, `religion`, `address`, `phone`, `email`, `learn_year`, `learn_status`, `status`) VALUES
(1, 'Nguyễn Văn An', NULL, '2010-05-12', 'Male', 'Kinh', 'Không', '123 Lý Thường Kiệt, TP. Hồ Chí Minh', '0912345678', 'an.nguyen@example.com', '2021-2024', 'Đang học', 1),
(2, 'Trần Thị Bình', NULL, '2010-09-23', 'Female', 'Kinh', 'Phật giáo', '45 Nguyễn Trãi, TP. Hồ Chí Minh', '0923456789', 'binh.tran@example.com', '2022-2025', 'Đang học', 1),
(3, 'Lê Văn Cường', NULL, '2009-03-15', 'Male', 'Mường', 'Không', '67 Hai Bà Trưng, TP. Hồ Chí Minh', '0934567890', 'cuong.le@example.com', '2020-2023', 'Bảo lưu', 1),
(4, 'Phạm Thị Dung', NULL, '2011-07-08', 'Female', 'Kinh', 'Công giáo', '89 Điện Biên Phủ, TP. Hồ Chí Minh', '0945678901', 'dung.pham@example.com', '2021-2024', 'Đang học', 1),
(5, 'Hoàng Văn Em', NULL, '2011-11-30', 'Male', 'Thái', 'Không', '12 Cách Mạng Tháng Tám, TP. Hồ Chí Minh', '0956789012', 'em.hoang@example.com', '2022-2025', 'Đang học', 1),
(6, 'Đỗ Thị Hạnh', NULL, '2008-04-22', 'Female', 'Kinh', 'Phật giáo', '234 Nguyễn Văn Cừ, TP. Hồ Chí Minh', '0967890123', 'hanh.do@example.com', '2019-2020', 'Tốt nghiệp', 1),
(7, 'Bùi Văn Khánh', NULL, '2010-01-19', 'Male', 'Tày', 'Không', '56 Võ Văn Tần, TP. Hồ Chí Minh', '0978901234', 'khanh.bui@example.com', '2021-2022', 'Đang học', 1),
(8, 'Ngô Thị Lan', NULL, '2011-06-05', 'Female', 'Kinh', 'Không', '78 Trần Hưng Đạo, TP. Hồ Chí Minh', '0989012345', 'lan.ngo@example.com', '2022-2023', 'Nghỉ học', 1),
(9, 'Vũ Văn Minh', NULL, '2009-08-17', 'Male', 'Hoa', 'Không', '90 Pasteur, TP. Hồ Chí Minh', '0990123456', 'minh.vu@example.com', '2020-2021', 'Bảo lưu', 1),
(10, 'Phan Thị Ngọc', NULL, '2011-12-25', 'Female', 'Khmer', 'Không', '321 Nguyễn Thị Minh Khai, TP. Hồ Chí Minh', '0901234567', 'ngoc.phan@example.com', '2023-2024', 'Đang học', 1),
(11, 'Nguyễn Văn A', NULL, '2007-05-12', 'Male', 'Kinh', 'Không', '123 Lê Lợi, TP.HCM', '0901234567', 'nva@example.com', '2022-2025', 'Đang học', 1),
(12, 'Trần Thị B', NULL, '2008-03-22', 'Female', 'Kinh', 'Phật giáo', '456 Nguyễn Huệ, TP.HCM', '0902345678', 'ttb@example.com', '2022-2025', 'Đang học', 1),
(13, 'Lê Văn C', NULL, '2007-11-30', 'Male', 'Kinh', 'Thiên chúa', '789 Hai Bà Trưng, TP.HCM', '0903456789', 'lvc@example.com', '2022-2025', 'Đang học', 1),
(14, 'Phạm Thị D', NULL, '2008-07-15', 'Female', 'Kinh', 'Không', '321 Trần Hưng Đạo, TP.HCM', '0904567890', 'ptd@example.com', '2022-2025', 'Đang học', 1),
(15, 'Hoàng Văn E', NULL, '2007-09-05', 'Male', 'Kinh', 'Không', '654 Võ Văn Tần, TP.HCM', '0905678901', 'hve@example.com', '2022-2025', 'Đang học', 1),
(16, 'Đặng Thị F', NULL, '2008-01-18', 'Female', 'Kinh', 'Phật giáo', '987 Nguyễn Thị Minh Khai, TP.HCM', '0906789012', 'dtf@example.com', '2022-2025', 'Đang học', 1),
(17, 'Vũ Văn G', NULL, '2007-06-25', 'Male', 'Kinh', 'Không', '159 Cách Mạng Tháng 8, TP.HCM', '0907890123', 'vvg@example.com', '2022-2025', 'Đang học', 1),
(18, 'Bùi Thị H', NULL, '2008-10-10', 'Female', 'Kinh', 'Thiên chúa', '753 Điện Biên Phủ, TP.HCM', '0908901234', 'bth@example.com', '2022-2025', 'Đang học', 1),
(19, 'Ngô Văn I', NULL, '2007-12-01', 'Male', 'Kinh', 'Không', '852 Nguyễn Đình Chiểu, TP.HCM', '0909012345', 'nvi@example.com', '2022-2025', 'Đang học', 1),
(20, 'Dương Thị J', NULL, '2008-04-09', 'Female', 'Kinh', 'Phật giáo', '951 Lý Tự Trọng, TP.HCM', '0910123456', 'dtj@example.com', '2022-2025', 'Đang học', 1),
(21, 'Tạ Văn K', NULL, '2007-08-14', 'Male', 'Kinh', 'Không', '147 Pasteur, TP.HCM', '0911234567', 'tvk@example.com', '2022-2025', 'Đang học', 1),
(22, 'Lâm Thị L', NULL, '2008-02-28', 'Female', 'Kinh', 'Thiên chúa', '369 Nam Kỳ Khởi Nghĩa, TP.HCM', '0912345678', 'ltl@example.com', '2022-2025', 'Đang học', 1),
(23, 'Mai Văn M', NULL, '2007-10-20', 'Male', 'Kinh', 'Không', '258 Nguyễn Trãi, TP.HCM', '0913456789', 'mvm@example.com', '2022-2025', 'Đang học', 1),
(24, 'Thảo Thị N', NULL, '2008-06-06', 'Female', 'Kinh', 'Phật giáo', '147 Lý Chính Thắng, TP.HCM', '0914567890', 'ttn@example.com', '2022-2025', 'Đang học', 1),
(25, 'Quách Văn O', NULL, '2007-03-17', 'Male', 'Kinh', 'Không', '369 Trương Định, TP.HCM', '0915678901', 'qvo@example.com', '2022-2025', 'Đang học', 1),
(26, 'Hồ Thị P', NULL, '2008-09-29', 'Female', 'Kinh', 'Thiên chúa', '753 Nguyễn Văn Cừ, TP.HCM', '0916789012', 'htp@example.com', '2022-2025', 'Đang học', 1),
(27, 'Lương Văn Q', NULL, '2007-01-03', 'Male', 'Kinh', 'Không', '852 Tôn Đức Thắng, TP.HCM', '0917890123', 'lvq@example.com', '2022-2025', 'Đang học', 1),
(28, 'Phan Thị R', NULL, '2008-12-12', 'Female', 'Kinh', 'Phật giáo', '951 Bà Huyện Thanh Quan, TP.HCM', '0918901234', 'ptr@example.com', '2022-2025', 'Đang học', 1),
(29, 'Trịnh Văn S', NULL, '2007-07-07', 'Male', 'Kinh', 'Không', '147 Nguyễn Phi Khanh, TP.HCM', '0919012345', 'tvs@example.com', '2022-2025', 'Đang học', 1),
(30, 'Lý Thị T', NULL, '2008-05-01', 'Female', 'Kinh', 'Thiên chúa', '369 Nguyễn Văn Thủ, TP.HCM', '0920123456', 'ltt@example.com', '2022-2025', 'Đang học', 1),
(31, 'Nguyễn Thị U', NULL, '2008-03-11', 'Female', 'Kinh', 'Không', '101 Nguyễn Văn Đậu, TP.HCM', '0921234567', 'ntu@example.com', '2022-2025', 'Đang học', 1),
(32, 'Trần Văn V', NULL, '2007-06-19', 'Male', 'Kinh', 'Phật giáo', '202 Phan Đăng Lưu, TP.HCM', '0922345678', 'tvv@example.com', '2022-2025', 'Đang học', 1),
(33, 'Lê Thị W', NULL, '2008-09-23', 'Female', 'Kinh', 'Thiên chúa', '303 Hoàng Hoa Thám, TP.HCM', '0923456789', 'ltw@example.com', '2022-2025', 'Đang học', 1),
(34, 'Phạm Văn X', NULL, '2007-12-05', 'Male', 'Kinh', 'Không', '404 Phan Xích Long, TP.HCM', '0924567890', 'pvx@example.com', '2022-2025', 'Đang học', 1),
(35, 'Hoàng Thị Y', NULL, '2008-01-17', 'Female', 'Kinh', 'Phật giáo', '505 Trường Sa, TP.HCM', '0925678901', 'hty@example.com', '2022-2025', 'Đang học', 1),
(36, 'Đặng Văn Z', NULL, '2007-04-28', 'Male', 'Kinh', 'Không', '606 Hoàng Sa, TP.HCM', '0926789012', 'dvz@example.com', '2022-2025', 'Đang học', 1),
(37, 'Vũ Thị AA', NULL, '2008-07-09', 'Female', 'Kinh', 'Thiên chúa', '707 Nguyễn Kiệm, TP.HCM', '0927890123', 'vtaa@example.com', '2022-2025', 'Đang học', 1),
(38, 'Bùi Văn BB', NULL, '2007-10-30', 'Male', 'Kinh', 'Không', '808 Nguyễn Oanh, TP.HCM', '0928901234', 'bvbb@example.com', '2022-2025', 'Đang học', 1),
(39, 'Ngô Thị CC', NULL, '2008-02-14', 'Female', 'Kinh', 'Phật giáo', '909 Quang Trung, TP.HCM', '0929012345', 'ntcc@example.com', '2022-2025', 'Đang học', 1),
(40, 'Dương Văn DD', NULL, '2007-08-21', 'Male', 'Kinh', 'Không', '111 Phạm Văn Chiêu, TP.HCM', '0930123456', 'dvdd@example.com', '2022-2025', 'Đang học', 1),
(41, 'Tạ Thị EE', NULL, '2008-05-03', 'Female', 'Kinh', 'Thiên chúa', '222 Lê Đức Thọ, TP.HCM', '0931234567', 'ttee@example.com', '2022-2025', 'Đang học', 1),
(42, 'Lâm Văn FF', NULL, '2007-11-16', 'Male', 'Kinh', 'Không', '333 Thống Nhất, TP.HCM', '0932345678', 'lvff@example.com', '2022-2025', 'Đang học', 1),
(43, 'Mai Thị GG', NULL, '2008-06-27', 'Female', 'Kinh', 'Phật giáo', '444 Nguyễn Văn Lượng, TP.HCM', '0933456789', 'magg@example.com', '2022-2025', 'Đang học', 1),
(44, 'Thảo Văn HH', NULL, '2007-09-08', 'Male', 'Kinh', 'Không', '555 Lê Văn Thọ, TP.HCM', '0934567890', 'tvhh@example.com', '2022-2025', 'Đang học', 1),
(45, 'Quách Thị II', NULL, '2008-12-19', 'Female', 'Kinh', 'Thiên chúa', '666 Phan Huy Ích, TP.HCM', '0935678901', 'qtii@example.com', '2022-2025', 'Đang học', 1),
(46, 'Hồ Văn JJ', NULL, '2007-03-02', 'Male', 'Kinh', 'Không', '777 Trường Chinh, TP.HCM', '0936789012', 'hvjj@example.com', '2022-2025', 'Đang học', 1),
(47, 'Lương Thị KK', NULL, '2008-10-13', 'Female', 'Kinh', 'Phật giáo', '888 Âu Cơ, TP.HCM', '0937890123', 'ltkk@example.com', '2022-2025', 'Đang học', 1),
(48, 'Phan Văn LL', NULL, '2007-01-25', 'Male', 'Kinh', 'Không', '999 Lũy Bán Bích, TP.HCM', '0938901234', 'pvll@example.com', '2022-2025', 'Đang học', 1),
(49, 'Trịnh Thị MM', NULL, '2008-04-06', 'Female', 'Kinh', 'Thiên chúa', '121 Tân Kỳ Tân Quý, TP.HCM', '0939012345', 'ttmm@example.com', '2022-2025', 'Đang học', 1),
(50, 'Lý Văn NN', NULL, '2007-07-18', 'Male', 'Kinh', 'Không', '232 Tây Thạnh, TP.HCM', '0940123456', 'lvnn@example.com', '2022-2025', 'Đang học', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `student_exams`
--

CREATE TABLE `student_exams` (
  `examinee_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `exam_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `student_exams`
--

INSERT INTO `student_exams` (`examinee_id`, `student_id`, `exam_id`) VALUES
(1, 1, 1),
(2, 2, 1),
(3, 3, 2),
(4, 4, 2),
(5, 5, 3),
(6, 6, 3),
(7, 7, 4),
(8, 8, 4),
(9, 9, 5),
(10, 10, 5),
(11, 11, 6),
(12, 12, 6),
(13, 13, 7),
(14, 14, 7),
(15, 15, 8),
(16, 16, 8),
(17, 17, 9),
(18, 18, 9),
(19, 19, 10),
(20, 20, 10);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `subjects`
--

CREATE TABLE `subjects` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `subjects`
--

INSERT INTO `subjects` (`id`, `name`, `description`, `status`) VALUES
(1, 'Toán học', 'Môn học về các con số, phép tính, hình học và tư duy logic.', 1),
(2, 'Ngữ văn', 'Môn học về tiếng Việt, văn học, kỹ năng đọc hiểu và viết luận.', 1),
(3, 'Tiếng Anh', 'Môn học về ngôn ngữ tiếng Anh, bao gồm từ vựng, ngữ pháp và giao tiếp.', 1),
(4, 'Vật lý', 'Môn học nghiên cứu các hiện tượng tự nhiên như chuyển động, lực, năng lượng.', 1),
(5, 'Hóa học', 'Môn học về cấu tạo, tính chất và sự biến đổi của chất.', 1),
(6, 'Sinh học', 'Môn học về cơ thể sống, sinh vật và môi trường sống.', 1),
(7, 'Lịch sử', 'Môn học về các sự kiện, nhân vật và quá trình phát triển của xã hội loài người.', 1),
(8, 'Địa lý', 'Môn học về vị trí địa lý, khí hậu, dân cư và các vùng lãnh thổ.', 1),
(9, 'Tin học', 'Môn học về máy tính, phần mềm, lập trình và ứng dụng công nghệ thông tin.', 1),
(10, 'Giáo dục công dân', 'Môn học về quyền và nghĩa vụ công dân, đạo đức và pháp luật.', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `subject_term_avg`
--

CREATE TABLE `subject_term_avg` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `score` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `subject_term_avg`
--

-- INSERT INTO `subject_term_avg` (`assign_class_id`, `student_id`, `subject_id`, `score`) VALUES
-- (11, 1, 1, 8.00),  -- Thêm dữ liệu cho student 1 để nhất quán (giả sử tương tự)
-- (11, 21, 1, 8.00),
-- (12, 2, 1, 7.50),
-- (12, 2, 2, 8.50),
-- (12, 3, 1, 8.50),
-- (12, 3, 2, 8.00),
-- (12, 4, 1, 9.50),
-- (12, 4, 2, 8.00),
-- (12, 5, 1, 7.50),
-- (12, 5, 2, 7.50),
-- (12, 6, 1, 8.00),
-- (12, 6, 2, 7.75),
-- (12, 7, 1, 8.00),
-- (12, 7, 2, 9.00),
-- (13, 23, 3, 6.80),
-- (14, 24, 4, 9.10),
-- (15, 25, 5, 8.60),
-- (16, 26, 6, 7.90),
-- (17, 27, 7, 6.70),
-- (18, 28, 8, 8.40),
-- (19, 29, 9, 9.00),
-- (20, 30, 10, 7.85);

INSERT INTO subject_term_avg (assign_class_id, student_id, subject_id, score) VALUES
(12, 2, 1, 8.07),
(12, 2, 2, 8.75),
(12, 3, 1, 8.14),
(12, 3, 2, 8.18),
(12, 4, 1, 7.86),
(12, 4, 2, 8.64),
(12, 5, 1, 8.14),
(12, 5, 2, 8.38),
(12, 6, 1, 8.36),
(12, 6, 2, 8.61),
(12, 7, 1, 7.36),
(12, 7, 2, 7.50);


-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `teachers`
--

CREATE TABLE `teachers` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `gender` enum('Nam','Nữ') DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `phone` varchar(10) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `teachers`
--

INSERT INTO teachers
(id, fullname, avatar, birthday, gender, address, phone, email, status, user_id)
VALUES
(1, 'Nguyễn Văn Hùng', NULL, '1980-03-15', 'Nam', '123 Lê Lợi, TP.HCM', '0901234567', 'nvhung@example.com', 1, 5),
(2, 'Trần Thị Mai', NULL, '1982-07-22', 'Nữ', '456 Nguyễn Huệ, TP.HCM', '0902345678', 'ttmai@example.com', 1, 6),
(3, 'Lê Văn Phúc', NULL, '1979-11-30', 'Nam', '789 Hai Bà Trưng, TP.HCM', '0903456789', 'lvphuc@example.com', 1, 7),
(4, 'Phạm Thị Hương', NULL, '1985-05-10', 'Nữ', '321 Trần Hưng Đạo, TP.HCM', '0904567890', 'pthuong@example.com', 1, 8),
(5, 'Hoàng Văn Tâm', NULL, '1981-09-05', 'Nam', '654 Võ Văn Tần, TP.HCM', '0905678901', 'hvtam@example.com', 1, 9),
(6, 'Đặng Thị Lan', NULL, '1983-01-18', 'Nữ', '987 Nguyễn Thị Minh Khai, TP.HCM', '0906789012', 'dtlan@example.com', 1, 10),
(7, 'Vũ Văn Quang', NULL, '1978-06-25', 'Nam', '159 Cách Mạng Tháng 8, TP.HCM', '0907890123', 'vvquang@example.com', 1, 11),
(8, 'Bùi Thị Ngọc', NULL, '1984-10-10', 'Nữ', '753 Điện Biên Phủ, TP.HCM', '0908901234', 'btngoc@example.com', 1, 12),
(9, 'Ngô Văn Sơn', NULL, '1980-12-01', 'Nam', '852 Nguyễn Đình Chiểu, TP.HCM', '0909012345', 'nvson@example.com', 1, 13),
(10, 'Dương Thị Thu', NULL, '1986-04-09', 'Nữ', '951 Lý Tự Trọng, TP.HCM', '0910123456', 'dtthu@example.com', 1, 14);
-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `terms`
--

CREATE TABLE `terms` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `year` int(11) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `terms`
--

INSERT INTO `terms` (`id`, `name`, `year`, `start_date`, `end_date`, `status`) VALUES
(1, 'Học kỳ 1', 2021, '2021-09-01', '2022-01-15', 1),
(2, 'Học kỳ 2', 2021, '2022-02-01', '2022-06-15', 1),
(3, 'Học kỳ 1', 2022, '2022-09-01', '2023-01-15', 1),
(4, 'Học kỳ 2', 2022, '2023-02-01', '2023-06-15', 1),
(5, 'Học kỳ 1', 2023, '2023-09-01', '2024-01-15', 1),
(6, 'Học kỳ 2', 2023, '2024-02-01', '2024-06-15', 1),
(7, 'Học kỳ 1', 2024, '2024-09-01', '2025-01-15', 1),
(8, 'Học kỳ 2', 2024, '2025-02-01', '2025-06-15', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `term_gpa`
--

CREATE TABLE `term_gpa` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `gpa` decimal(10,2) DEFAULT NULL,
  `conduct_level` enum('Good','Fair','Satisfactory','Unsatisfactory') DEFAULT NULL,
  `academic` enum('Good','Fair','Satisfactory','Unsatisfactory') DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `term_gpa`
--

INSERT INTO term_gpa (assign_class_id, student_id, gpa, conduct_level, academic) VALUES
(12, 2, 8.41, 'Good', 'Good'),
(12, 3, 8.16, 'Good', 'Good'),
(12, 4, 8.25, 'Good', 'Good'),
(12, 5, 8.26, 'Good', 'Good'),
(12, 6, 8.49, 'Good', 'Good'),
(12, 7, 7.43, 'Fair', 'Fair');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `tuitions`
--

CREATE TABLE `tuitions` (
  `id` int(11) NOT NULL,
  `assign_class_id` int(11) NOT NULL,
  `user_id` int(11) NOT NULL,
  `time_create` datetime DEFAULT NULL,
  `amount` decimal(10,2) NOT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `tuitions`
--

INSERT INTO `tuitions` (`id`, `assign_class_id`, `user_id`, `time_create`, `amount`, `status`) VALUES
(1, 11, 1, '2025-09-01 08:00:00', 1200000.00, 1),
(2, 12, 2, '2025-09-01 08:05:00', 1200000.00, 1),
(3, 13, 1, '2025-09-01 08:10:00', 1200000.00, 1),
(4, 14, 1, '2025-09-02 09:00:00', 1250000.00, 1),
(5, 15, 1, '2025-09-02 09:10:00', 1250000.00, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `tuition_details`
--

CREATE TABLE `tuition_details` (
  `tuition_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `time_create` datetime DEFAULT NULL,
  `price` decimal(10,2) NOT NULL,
  `status` enum('Paid','Unpaid') DEFAULT 'Unpaid'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `tuition_details`
--

INSERT INTO `tuition_details` (`tuition_id`, `student_id`, `time_create`, `price`, `status`) VALUES
(1, 1, '2025-09-01 08:00:00', 1200000.00, 'Paid'),
(1, 2, '2025-09-01 08:05:00', 1200000.00, 'Paid'),
(2, 3, '2025-09-02 09:00:00', 1250000.00, 'Unpaid'),
(2, 4, '2025-09-02 09:10:00', 1250000.00, 'Paid'),
(3, 5, '2025-09-03 10:00:00', 1300000.00, 'Paid'),
(3, 6, '2025-09-03 10:15:00', 1300000.00, 'Unpaid'),
(4, 7, '2025-09-04 11:00:00', 1350000.00, 'Paid'),
(4, 8, '2025-09-04 11:20:00', 1350000.00, 'Paid'),
(5, 9, '2025-09-05 13:00:00', 1400000.00, 'Unpaid'),
(5, 10, '2025-09-05 13:15:00', 1400000.00, 'Paid');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `role_id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `fullname` varchar(100) DEFAULT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `phone` varchar(15) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `status` varchar(100) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `users`
--

INSERT INTO `users` (`id`, `role_id`, `username`, `password`, `fullname`, `avatar`, `phone`, `email`, `address`, `status`) VALUES
(1, 1, 'admin01', '123456', 'Nguyễn Văn Quản Trị', NULL, '0909123456', 'admin@example.com', '232 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
(2, 2, 'gv01', '123456', 'Trần Thị Giáo Viên', NULL, '0909234567', 'giaovien@example.com', '232 hưng phú phường chợ lớn Tp.HCM', 'Hoạt động'),
(4, 1, 'abc', 'abc123', 'Nguyễn Văn A', '', '0967548341', 'abc@gmail.com', '1', 'Hoạt động'),
(5, 2, 'nvhung', '123456', 'Nguyễn Văn Hùng', NULL, '0901234567', 'nvhung@example.com', '123 Lê Lợi, TP.HCM', 'Hoạt động'),
(6, 2, 'ttmai', '123456', 'Trần Thị Mai', NULL, '0902345678', 'ttmai@example.com', '456 Nguyễn Huệ, TP.HCM', 'Hoạt động'),
(7, 2, 'lvphuc', '123456', 'Lê Văn Phúc', NULL, '0903456789', 'lvphuc@example.com', '789 Hai Bà Trưng, TP.HCM', 'Hoạt động'),
(8, 2, 'pthuong', '123456', 'Phạm Thị Hương', NULL, '0904567890', 'pthuong@example.com', '321 Trần Hưng Đạo, TP.HCM', 'Hoạt động'),
(9, 2, 'hvtam', '123456', 'Hoàng Văn Tâm', NULL, '0905678901', 'hvtam@example.com', '654 Võ Văn Tần, TP.HCM', 'Hoạt động'),
(10, 2, 'dtlan', '123456', 'Đặng Thị Lan', NULL, '0906789012', 'dtlan@example.com', '987 Nguyễn Thị Minh Khai, TP.HCM', 'Hoạt động'),
(11, 2, 'vvquang', '123456', 'Vũ Văn Quang', NULL, '0907890123', 'vvquang@example.com', '159 Cách Mạng Tháng 8, TP.HCM', 'Hoạt động'),
(12, 2, 'btngoc', '123456', 'Bùi Thị Ngọc', NULL, '0908901234', 'btngoc@example.com', '753 Điện Biên Phủ, TP.HCM', 'Hoạt động'),
(13, 2, 'nvson', '123456', 'Ngô Văn Sơn', NULL, '0909012345', 'nvson@example.com', '852 Nguyễn Đình Chiểu, TP.HCM', 'Hoạt động'),
(14, 2, 'dtthu', '123456', 'Dương Thị Thu', NULL, '0910123456', 'dtthu@example.com', '951 Lý Tự Trọng, TP.HCM', 'Hoạt động');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `violations`
--

CREATE TABLE `violations` (
  `id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `assign_class_id` int(11) NOT NULL,
  `rule_id` int(11) DEFAULT NULL,
  `date_create` date DEFAULT NULL,
  `description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Đang đổ dữ liệu cho bảng `violations`
--

INSERT INTO `violations` (`id`, `student_id`, `assign_class_id`, `rule_id`, `date_create`, `description`) VALUES
(1, 1, 11, 1, '2025-09-15', 'Đi học trễ 30 phút không có lý do.'),
(2, 2, 11, 2, '2025-09-16', 'Không mặc đồng phục theo quy định.'),
(3, 3, 12, 3, '2025-09-17', 'Xả rác trong lớp học.'),
(4, 4, 12, 4, '2025-09-18', 'Sử dụng điện thoại trong giờ học.'),
(5, 5, 13, 5, '2025-09-19', 'Nói chuyện hỗn với giáo viên.'),
(6, 6, 13, 6, '2025-09-20', 'Gây mất trật tự trong giờ kiểm tra.'),
(7, 7, 14, 7, '2025-09-21', 'Không tham gia buổi sinh hoạt lớp.'),
(8, 8, 14, 8, '2025-09-22', 'Mang thuốc lá vào trường.'),
(9, 9, 15, 9, '2025-09-23', 'Xô xát với bạn cùng lớp sau giờ học.'),
(10, 10, 15, 10, '2025-09-24', 'Làm hỏng thiết bị máy chiếu của lớp.');

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `assign_classes`
--
ALTER TABLE `assign_classes`
  ADD PRIMARY KEY (`id`,`class_id`,`term_id`,`head_teacher_id`),
  ADD KEY `class_id` (`class_id`),
  ADD KEY `head_teacher_id` (`head_teacher_id`),
  ADD KEY `term_id` (`term_id`);

--
-- Chỉ mục cho bảng `assign_class_students`
--
ALTER TABLE `assign_class_students`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Chỉ mục cho bảng `assign_class_teachers`
--
ALTER TABLE `assign_class_teachers`
  ADD PRIMARY KEY (`assign_class_id`,`teacher_id`,`subject_id`,`day`),
  ADD KEY `teacher_id` (`teacher_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Chỉ mục cho bảng `classes`
--
ALTER TABLE `classes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `class_type_id` (`class_type_id`);

--
-- Chỉ mục cho bảng `class_types`
--
ALTER TABLE `class_types`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `departments`
--
ALTER TABLE `departments`
  ADD PRIMARY KEY (`id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Chỉ mục cho bảng `department_details`
--
ALTER TABLE `department_details`
  ADD PRIMARY KEY (`department_id`,`teacher_id`),
  ADD KEY `teacher_id` (`teacher_id`);

--
-- Chỉ mục cho bảng `exams`
--
ALTER TABLE `exams`
  ADD PRIMARY KEY (`id`),
  ADD KEY `exam_detail_id` (`exam_detail_id`);

--
-- Chỉ mục cho bảng `exam_details`
--
ALTER TABLE `exam_details`
  ADD PRIMARY KEY (`id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `term_id` (`term_id`);

--
-- Chỉ mục cho bảng `exam_types`
--
ALTER TABLE `exam_types`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `functions`
--
ALTER TABLE `functions`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `relations`
--
ALTER TABLE `relations`
  ADD PRIMARY KEY (`id`),
  ADD KEY `student_id` (`student_id`);

--
-- Chỉ mục cho bảng `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `role_details`
--
ALTER TABLE `role_details`
  ADD PRIMARY KEY (`role_id`,`function_id`,`action`),
  ADD KEY `function_id` (`function_id`);

--
-- Chỉ mục cho bảng `rules`
--
ALTER TABLE `rules`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `score_details`
--
ALTER TABLE `score_details`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`,`subject_id`,`exam_type_id`,`attempt`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `exam_type_id` (`exam_type_id`);

--
-- Chỉ mục cho bảng `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `student_exams`
--
ALTER TABLE `student_exams`
  ADD PRIMARY KEY (`examinee_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `exam_id` (`exam_id`);

--
-- Chỉ mục cho bảng `subjects`
--
ALTER TABLE `subjects`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `subject_term_avg`
--
ALTER TABLE `subject_term_avg`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`,`subject_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Chỉ mục cho bảng `teachers`
--
ALTER TABLE `teachers`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`);

--
-- Chỉ mục cho bảng `terms`
--
ALTER TABLE `terms`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `term_gpa`
--
ALTER TABLE `term_gpa`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Chỉ mục cho bảng `tuitions`
--
ALTER TABLE `tuitions`
  ADD PRIMARY KEY (`id`),
  ADD KEY `assign_class_id` (`assign_class_id`),
  ADD KEY `user_id` (`user_id`);

--
-- Chỉ mục cho bảng `tuition_details`
--
ALTER TABLE `tuition_details`
  ADD PRIMARY KEY (`tuition_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Chỉ mục cho bảng `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`),
  ADD KEY `role_id` (`role_id`);

--
-- Chỉ mục cho bảng `violations`
--
ALTER TABLE `violations`
  ADD PRIMARY KEY (`id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `assign_class_id` (`assign_class_id`),
  ADD KEY `rule_id` (`rule_id`);

--
-- AUTO_INCREMENT cho các bảng đã đổ
--

--
-- AUTO_INCREMENT cho bảng `assign_classes`
--
ALTER TABLE `assign_classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT cho bảng `classes`
--
ALTER TABLE `classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT cho bảng `class_types`
--
ALTER TABLE `class_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT cho bảng `departments`
--
ALTER TABLE `departments`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `exams`
--
ALTER TABLE `exams`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `exam_details`
--
ALTER TABLE `exam_details`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `exam_types`
--
ALTER TABLE `exam_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT cho bảng `functions`
--
ALTER TABLE `functions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT cho bảng `relations`
--
ALTER TABLE `relations`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT cho bảng `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT cho bảng `rules`
--
ALTER TABLE `rules`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `students`
--
ALTER TABLE `students`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT cho bảng `student_exams`
--
ALTER TABLE `student_exams`
  MODIFY `examinee_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT cho bảng `subjects`
--
ALTER TABLE `subjects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `teachers`
--
ALTER TABLE `teachers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT cho bảng `terms`
--
ALTER TABLE `terms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT cho bảng `tuitions`
--
ALTER TABLE `tuitions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT cho bảng `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT cho bảng `violations`
--
ALTER TABLE `violations`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- Các ràng buộc cho các bảng đã đổ
--

--
-- Các ràng buộc cho bảng `assign_classes`
--
ALTER TABLE `assign_classes`
  ADD CONSTRAINT `assign_classes_ibfk_1` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`),
  ADD CONSTRAINT `assign_classes_ibfk_2` FOREIGN KEY (`head_teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `assign_classes_ibfk_3` FOREIGN KEY (`term_id`) REFERENCES `terms` (`id`);

--
-- Các ràng buộc cho bảng `assign_class_students`
--
ALTER TABLE `assign_class_students`
  ADD CONSTRAINT `assign_class_students_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `assign_class_students_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Các ràng buộc cho bảng `assign_class_teachers`
--
ALTER TABLE `assign_class_teachers`
  ADD CONSTRAINT `assign_class_teachers_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `assign_class_teachers_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `assign_class_teachers_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Các ràng buộc cho bảng `classes`
--
ALTER TABLE `classes`
  ADD CONSTRAINT `classes_ibfk_1` FOREIGN KEY (`class_type_id`) REFERENCES `class_types` (`id`);

--
-- Các ràng buộc cho bảng `departments`
--
ALTER TABLE `departments`
  ADD CONSTRAINT `departments_ibfk_1` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Các ràng buộc cho bảng `department_details`
--
ALTER TABLE `department_details`
  ADD CONSTRAINT `department_details_ibfk_1` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`),
  ADD CONSTRAINT `department_details_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`);

--
-- Các ràng buộc cho bảng `exams`
--
ALTER TABLE `exams`
  ADD CONSTRAINT `exams_ibfk_1` FOREIGN KEY (`exam_detail_id`) REFERENCES `exam_details` (`id`);

--
-- Các ràng buộc cho bảng `exam_details`
--
ALTER TABLE `exam_details`
  ADD CONSTRAINT `exam_details_ibfk_1` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`),
  ADD CONSTRAINT `exam_details_ibfk_2` FOREIGN KEY (`term_id`) REFERENCES `terms` (`id`);

--
-- Các ràng buộc cho bảng `relations`
--
ALTER TABLE `relations`
  ADD CONSTRAINT `relations_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Các ràng buộc cho bảng `role_details`
--
ALTER TABLE `role_details`
  ADD CONSTRAINT `role_details_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`),
  ADD CONSTRAINT `role_details_ibfk_2` FOREIGN KEY (`function_id`) REFERENCES `functions` (`id`);

--
-- Các ràng buộc cho bảng `score_details`
--
ALTER TABLE `score_details`
  ADD CONSTRAINT `score_details_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `score_details_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `score_details_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`),
  ADD CONSTRAINT `score_details_ibfk_4` FOREIGN KEY (`exam_type_id`) REFERENCES `exam_types` (`id`);

--
-- Các ràng buộc cho bảng `student_exams`
--
ALTER TABLE `student_exams`
  ADD CONSTRAINT `student_exams_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `student_exams_ibfk_2` FOREIGN KEY (`exam_id`) REFERENCES `exams` (`id`);

--
-- Các ràng buộc cho bảng `subject_term_avg`
--
ALTER TABLE `subject_term_avg`
  ADD CONSTRAINT `subject_term_avg_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `subject_term_avg_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `subject_term_avg_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Các ràng buộc cho bảng `teachers`
--
ALTER TABLE `teachers`
  ADD CONSTRAINT `teachers_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);

--
-- Các ràng buộc cho bảng `term_gpa`
--
ALTER TABLE `term_gpa`
  ADD CONSTRAINT `term_gpa_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `term_gpa_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);


--
-- Các ràng buộc cho bảng `tuitions`
--
ALTER TABLE `tuitions`
  ADD CONSTRAINT `tuitions_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `tuitions_ibfk_2` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);

--
-- Các ràng buộc cho bảng `tuition_details`
--
ALTER TABLE `tuition_details`
  ADD CONSTRAINT `tuition_details_ibfk_1` FOREIGN KEY (`tuition_id`) REFERENCES `tuitions` (`id`),
  ADD CONSTRAINT `tuition_details_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Các ràng buộc cho bảng `users`
--
ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`);

--
-- Các ràng buộc cho bảng `violations`
--
ALTER TABLE `violations`
  ADD CONSTRAINT `violations_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `violations_ibfk_2` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `violations_ibfk_3` FOREIGN KEY (`rule_id`) REFERENCES `rules` (`id`);

-- cập nhật trung bình môn
DELIMITER //

-- ============================================
-- Trigger 1: Cập nhật subject_term_avg khi INSERT score_details
-- ============================================
DROP TRIGGER IF EXISTS calc_subject_avg_after_insert//
CREATE TRIGGER calc_subject_avg_after_insert
AFTER INSERT ON score_details 
FOR EACH ROW
BEGIN
    REPLACE INTO subject_term_avg (assign_class_id, student_id, subject_id, score)
    SELECT
        assign_class_id,
        student_id,
        subject_id,
        SUM(score *
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END)
        /
        SUM(
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END) AS avg_score
    FROM score_details
    WHERE assign_class_id = NEW.assign_class_id
    AND student_id = NEW.student_id
    AND subject_id = NEW.subject_id
    GROUP BY assign_class_id, student_id, subject_id;
END //

-- ============================================
-- Trigger 2: Cập nhật subject_term_avg khi UPDATE score_details
-- ============================================
DROP TRIGGER IF EXISTS calc_subject_avg_after_update//
CREATE TRIGGER calc_subject_avg_after_update
AFTER UPDATE ON score_details 
FOR EACH ROW
BEGIN
    REPLACE INTO subject_term_avg (assign_class_id, student_id, subject_id, score)
    SELECT
        assign_class_id,
        student_id,
        subject_id,
        SUM(score *
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END)
        /
        SUM(
            CASE exam_type_id
                WHEN 1 THEN 1
                WHEN 2 THEN 1
                WHEN 3 THEN 2
                WHEN 4 THEN 3
            END) AS avg_score
    FROM score_details
    WHERE assign_class_id = NEW.assign_class_id
    AND student_id = NEW.student_id
    AND subject_id = NEW.subject_id
    GROUP BY assign_class_id, student_id, subject_id;
END //

-- ============================================
-- Trigger 3: Cập nhật subject_term_avg khi DELETE score_details
-- ============================================
DROP TRIGGER IF EXISTS calc_subject_avg_after_delete//
CREATE TRIGGER calc_subject_avg_after_delete
AFTER DELETE ON score_details 
FOR EACH ROW
BEGIN
    DECLARE remaining_count INT;
    
    SELECT COUNT(*)
    INTO remaining_count
    FROM score_details
    WHERE assign_class_id = OLD.assign_class_id
    AND student_id = OLD.student_id
    AND subject_id = OLD.subject_id;
    
    IF remaining_count > 0 THEN
        REPLACE INTO subject_term_avg (assign_class_id, student_id, subject_id, score)
        SELECT
            assign_class_id,
            student_id,
            subject_id,
            SUM(score *
                CASE exam_type_id
                    WHEN 1 THEN 1
                    WHEN 2 THEN 1
                    WHEN 3 THEN 2
                    WHEN 4 THEN 3
                END)
            /
            SUM(
                CASE exam_type_id
                    WHEN 1 THEN 1
                    WHEN 2 THEN 1
                    WHEN 3 THEN 2
                    WHEN 4 THEN 3
                END) AS avg_score
        FROM score_details
        WHERE assign_class_id = OLD.assign_class_id
        AND student_id = OLD.student_id
        AND subject_id = OLD.subject_id
        GROUP BY assign_class_id, student_id, subject_id;
    ELSE
        DELETE FROM subject_term_avg
        WHERE assign_class_id = OLD.assign_class_id
        AND student_id = OLD.student_id
        AND subject_id = OLD.subject_id;
    END IF;
END //

-- ============================================
-- Trigger 4: Cập nhật term_gpa khi INSERT subject_term_avg
-- ============================================
DROP TRIGGER IF EXISTS trg_gpa_after_insert//
CREATE TRIGGER trg_gpa_after_insert
AFTER INSERT ON subject_term_avg
FOR EACH ROW
BEGIN
    DECLARE gpa_val FLOAT;
    DECLARE current_conduct VARCHAR(50);

    SELECT AVG(score)
    INTO gpa_val
    FROM subject_term_avg
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    SELECT conduct_level
    INTO current_conduct
    FROM term_gpa
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    REPLACE INTO term_gpa(assign_class_id, student_id, gpa, conduct_level, academic)
    VALUES(
        NEW.assign_class_id,
        NEW.student_id,
        gpa_val,
        current_conduct,
        CASE
            WHEN current_conduct IS NULL THEN NULL
            WHEN gpa_val < 5 OR current_conduct = 'Unsatisfactory' THEN 'Unsatisfactory' 
            WHEN gpa_val >= 8 AND current_conduct = 'Good' THEN 'Good' 
            WHEN gpa_val >= 8 AND current_conduct = 'Fair' THEN 'Fair' 
            WHEN gpa_val >= 6.5 AND current_conduct IN ('Fair', 'Good') THEN 'Fair' 
            ELSE 'Satisfactory'
        END
    );
END //

-- ============================================
-- Trigger 5: Cập nhật term_gpa khi UPDATE subject_term_avg
-- ============================================
DROP TRIGGER IF EXISTS trg_gpa_after_update//
CREATE TRIGGER trg_gpa_after_update
AFTER UPDATE ON subject_term_avg
FOR EACH ROW
BEGIN
    DECLARE gpa_val FLOAT;
    DECLARE current_conduct VARCHAR(50);

    SELECT AVG(score)
    INTO gpa_val
    FROM subject_term_avg
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    SELECT conduct_level
    INTO current_conduct
    FROM term_gpa
    WHERE assign_class_id = NEW.assign_class_id
      AND student_id = NEW.student_id;

    REPLACE INTO term_gpa(assign_class_id, student_id, gpa, conduct_level, academic)
    VALUES(
        NEW.assign_class_id,
        NEW.student_id,
        gpa_val,
        current_conduct,
        CASE
            WHEN current_conduct IS NULL THEN NULL
            WHEN gpa_val < 5 OR current_conduct = 'Unsatisfactory' THEN 'Unsatisfactory' 
            WHEN gpa_val >= 8 AND current_conduct = 'Good' THEN 'Good' 
            WHEN gpa_val >= 8 AND current_conduct = 'Fair' THEN 'Fair' 
            WHEN gpa_val >= 6.5 AND current_conduct IN ('Fair', 'Good') THEN 'Fair' 
            ELSE 'Satisfactory'
        END
    );
END //

-- ============================================
-- Trigger 6: Cập nhật term_gpa khi DELETE subject_term_avg
-- ============================================
DROP TRIGGER IF EXISTS trg_gpa_after_delete//
CREATE TRIGGER trg_gpa_after_delete
AFTER DELETE ON subject_term_avg
FOR EACH ROW
BEGIN
    DECLARE gpa_val FLOAT;
    DECLARE current_conduct VARCHAR(50);

    SELECT AVG(score)
    INTO gpa_val
    FROM subject_term_avg
    WHERE assign_class_id = OLD.assign_class_id
      AND student_id = OLD.student_id;

    IF gpa_val IS NOT NULL THEN
        SELECT conduct_level
        INTO current_conduct
        FROM term_gpa
        WHERE assign_class_id = OLD.assign_class_id
          AND student_id = OLD.student_id;

        REPLACE INTO term_gpa(assign_class_id, student_id, gpa, conduct_level, academic)
        VALUES(
            OLD.assign_class_id,
            OLD.student_id,
            gpa_val,
            current_conduct,
            CASE
                WHEN current_conduct IS NULL THEN NULL
                WHEN gpa_val < 5 OR current_conduct = 'Unsatisfactory' THEN 'Unsatisfactory' 
                WHEN gpa_val >= 8 AND current_conduct = 'Good' THEN 'Good' 
                WHEN gpa_val >= 8 AND current_conduct = 'Fair' THEN 'Fair' 
                WHEN gpa_val >= 6.5 AND current_conduct IN ('Fair', 'Good') THEN 'Fair' 
                ELSE 'Satisfactory'
            END
        );
    ELSE
        DELETE FROM term_gpa
        WHERE assign_class_id = OLD.assign_class_id
          AND student_id = OLD.student_id;
    END IF;
END //

DELIMITER ;
COMMIT;




/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;