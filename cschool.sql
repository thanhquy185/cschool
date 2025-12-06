
-- Database: `cschool`


DROP DATABASE IF EXISTS CSchool;
CREATE DATABASE CSchool;
USE CSchool;

-- --------------------------------------------------------

--
-- Table structure for table `assign_classes`
--

CREATE TABLE `assign_classes` (
  `id` int(11) NOT NULL,
  `class_id` int(11) NOT NULL,
  `head_teacher_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `assign_classes`
--

INSERT INTO `assign_classes` (`id`, `class_id`, `head_teacher_id`, `term_id`, `status`) VALUES
(11, 1, 1, 9, 1),
(12, 2, 2, 9, 1),
(13, 3, 3, 9, 1),
(14, 4, 4, 9, 1),
(15, 5, 5, 9, 1),
(16, 6, 6, 9, 1),
(17, 7, 7, 9, 1),
(18, 8, 8, 9, 1),
(19, 2, 9, 9, 1),
(20, 1, 10, 9, 1);

-- --------------------------------------------------------

--
-- Table structure for table `assign_class_students`
--

CREATE TABLE `assign_class_students` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `role` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `assign_class_students`
--

INSERT INTO `assign_class_students` (`assign_class_id`, `student_id`, `role`) VALUES
(11, 1, 'Lớp trưởng'),
(12, 2, 'Lớp phó học tập'),
(13, 3, 'Lớp phó lao động'),
(13, 5, 'Thành viên'),
(13, 6, 'Thành viên'),
(14, 4, 'Thành viên'),
(14, 7, 'Thành viên'),
(15, 9, 'Thành viên'),
(16, 8, 'Thành viên'),
(17, 10, 'Thành viên'),

(18, 11, 'Lớp trưởng'),
(19, 12, 'Lớp phó học tập'),
(20, 13, 'Lớp phó lao động'),
(11, 14, 'Thành viên'),
(11, 15, 'Thành viên'),
(12, 16, 'Thành viên'),
(12, 17, 'Thành viên'),
(13, 18, 'Thành viên'),
(13, 19, 'Thành viên'),
(14, 20, 'Thành viên'),

(15, 21, 'Lớp trưởng'),
(16, 22, 'Lớp phó học tập'),
(17, 23, 'Lớp phó lao động'),
(18, 24, 'Thành viên'),
(18, 25, 'Thành viên'),
(19, 26, 'Thành viên'),
(19, 27, 'Thành viên'),
(20, 28, 'Thành viên'),
(20, 29, 'Thành viên'),
(11, 30, 'Thành viên'),

(12, 31, 'Lớp trưởng'),
(13, 32, 'Lớp phó học tập'),
(14, 33, 'Lớp phó lao động'),
(15, 34, 'Thành viên'),
(15, 35, 'Thành viên'),
(16, 36, 'Thành viên'),
(16, 37, 'Thành viên'),
(17, 38, 'Thành viên'),
(17, 39, 'Thành viên'),
(18, 40, 'Thành viên'),

(19, 41, 'Lớp trưởng'),
(20, 42, 'Lớp phó học tập'),
(11, 43, 'Lớp phó lao động'),
(12, 44, 'Thành viên'),
(12, 45, 'Thành viên'),
(13, 46, 'Thành viên'),
(13, 47, 'Thành viên'),
(14, 48, 'Thành viên'),
(14, 49, 'Thành viên'),
(15, 50, 'Thành viên');


-- --------------------------------------------------------

--
-- Table structure for table `assign_class_teachers`
--

CREATE TABLE `assign_class_teachers` (
  `assign_class_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `quiz_connt` int(11) DEFAULT NULL,
  `oral_count` int(11) DEFAULT NULL,
  `day` date DEFAULT NULL,
  `start_period` int(11) DEFAULT NULL,
  `end_period` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `assign_class_teachers`
--

INSERT INTO `assign_class_teachers` (`assign_class_id`, `teacher_id`, `subject_id`, `quiz_connt`, `oral_count`, `day`, `start_period`, `end_period`) VALUES
(11, 1, 1, 2, 1, '2025-10-01', 1, 2),
(12, 2, 2, 3, 2, '2025-10-02', 3, 4),
(13, 3, 3, 1, 1, '2025-10-03', 2, 3),
(14, 4, 4, 2, 2, '2025-10-04', 4, 5),
(15, 5, 5, 3, 1, '2025-10-05', 1, 2),
(16, 6, 6, 2, 2, '2025-10-06', 3, 4),
(17, 7, 7, 1, 1, '2025-10-07', 2, 3),
(18, 8, 8, 2, 2, '2025-10-08', 4, 5),
(19, 9, 9, 3, 1, '2025-10-09', 1, 2),
(20, 10, 10, 2, 2, '2025-10-10', 3, 4);

-- --------------------------------------------------------

--
-- Table structure for table `classes`
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
-- Dumping data for table `classes`
--

INSERT INTO `classes` (`id`, `class_type_id`, `grade`, `name`, `area`, `room`, `status`) VALUES
(1, 2, 10, '10A', 'Khu A', 'Phòng 101', 1),
(2, 2, 10, '10B', 'Khu A', 'Phòng 102', 1),
(3, 2, 10, '10C', 'Khu B', 'Phòng 201', 1),
(4, 3, 11, '11A', 'Khu B', 'Phòng 202', 1),
(5, 3, 11, '11B', 'Khu C', 'Phòng 301', 1),
(6, 4, 12, '12A1', 'Khu C', 'Phòng 302', 1),
(7, 4, 12, '12B', 'Khu D', 'Phòng 401', 1),
(8, 7, 12, '12A2', 'Khu D', 'Phòng 405', 1);

-- --------------------------------------------------------

--
-- Table structure for table `class_types`
--

CREATE TABLE `class_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `class_types`
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
-- Table structure for table `departments`
--

CREATE TABLE `departments` (
  `id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `departments`
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
-- Table structure for table `department_details`
--

CREATE TABLE `department_details` (
  `department_id` int(11) NOT NULL,
  `teacher_id` int(11) NOT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `role` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `department_details`
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
-- Table structure for table `exams`
--

CREATE TABLE `exams` (
    `id` int(11) NOT NULL,
    `exam_detail_id` int(11) NOT NULL,      -- Tham chiếu đến ca thi
    `exam_room` int(11) NOT NULL,   -- Phòng thi
    `supervisor_id` int(11) DEFAULT NULL   -- Giáo viên coi thi (nếu có)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `exams`
--

INSERT INTO `exams` (`id`, `exam_detail_id`, `exam_room`, `supervisor_id`) VALUES
(1, 1, 1, 5),
(2, 2, 2, 7),
(3, 3, 3, 8),
(4, 4, 4, 6),
(5, 5, 5, 9);

-- --------------------------------------------------------

--
-- Table structure for table `rooms`
--

CREATE TABLE `rooms` (
    `id` int(11) NOT NULL,
    `name` varchar(50) NOT NULL,
    `quantity` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `rooms`
--

INSERT INTO `rooms` (`id`, `name`, `quantity`) VALUES
(1, 'Phòng A101', 10),
(2, 'Phòng A102', 10),
(3, 'Phòng B201', 10),
(4, 'Phòng B202', 10),
(5, 'Phòng C301', 10),
(6, 'Phòng C302', 10),
(7, 'Phòng D401', 10),
(8, 'Phòng D402', 10),
(9, 'Phòng E501', 10),
(10, 'Phòng E502', 10);

-- --------------------------------------------------------

--
-- Table structure for table `exam_details`
--

CREATE TABLE `exam_details` (
    `id` int(11) NOT NULL,
    `subject_id` int(11) NOT NULL,          -- Môn học được thi
    `term_id` int(11) NOT NULL,             -- Học kỳ / năm học
    `exam_type_id` int(11) DEFAULT NULL,    -- Loại bài thi: giữa kỳ, cuối kỳ (tham chiếu exam_types)
    `start_time` datetime NOT NULL,     -- Giờ bắt đầu thi
    `end_time` datetime NOT NULL,       -- Giờ kết thúc thi
    `status` TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `exam_details`
--

INSERT INTO `exam_details` (`id`, `subject_id`, `term_id`, `exam_type_id`, `start_time`, `end_time`) VALUES
(1, 1, 9, 3, '2025-11-01 08:00:00', '2025-11-01 09:30:00'),
(2, 2, 9, 3, '2025-11-02 08:00:00', '2025-11-02 09:30:00'),
(3, 3, 9, 4, '2025-11-03 08:00:00', '2025-11-03 09:30:00'),
(4, 4, 9, 4, '2025-11-04 08:00:00', '2025-11-04 09:30:00'),
(5, 1, 9, 3, '2025-11-05 08:00:00', '2025-11-05 09:30:00');

-- --------------------------------------------------------

--
-- Table structure for table `exam_types`
--

CREATE TABLE `exam_types` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `weight` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `exam_types`
--

INSERT INTO `exam_types` (`id`, `name`, `weight`) VALUES
(1, 'Kiểm tra miệng', 1),
(2, 'Kiểm tra 15 phút', 1),
(3, 'Thi giữa kỳ', 2),
(4, 'Thi cuối kỳ', 3);

-- --------------------------------------------------------

--
-- Table structure for table `functions`
--

CREATE TABLE `functions` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `icon` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `functions`
--

INSERT INTO `functions` (`id`, `name`, `icon`) VALUES
(1, 'Thêm', 'fa-plus'),
(2, 'Sửa', 'fa-edit'),
(3, 'Xóa', 'fa-trash'),
(4, 'Phân lớp', 'fa-users'),
(5, 'Nhập điểm', 'fa-keyboard');

-- --------------------------------------------------------

--
-- Table structure for table `relations`
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
-- Dumping data for table `relations`
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
-- Table structure for table `roles`
--

CREATE TABLE `roles` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `roles`
--

INSERT INTO `roles` (`id`, `name`, `status`) VALUES
(1, 'Quản trị viên', 1),
(2, 'Giáo viên', 1),
(3, 'Học sinh', 1),
(4, 'Phụ huynh', 1),
(5, 'Khách', 1);

-- --------------------------------------------------------

--
-- Table structure for table `role_details`
--

CREATE TABLE `role_details` (
  `role_id` int(11) NOT NULL,
  `function_id` int(11) NOT NULL,
  `action` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `role_details`
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
-- Table structure for table `rules`
--

CREATE TABLE `rules` (
  `id` int(11) NOT NULL,
  `title` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `rules`
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
-- Table structure for table `score_details`
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
-- Dumping data for table `score_details`
--

INSERT INTO `score_details` (`assign_class_id`, `student_id`, `subject_id`, `exam_type_id`, `attempt`, `score`) VALUES
(11, 11, 1, 1, 1, 8.50),
(12, 12, 1, 2, 1, 7.00),
(12, 13, 2, 3, 1, 6.75),
(12, 14, 2, 4, 1, 9.00),
(13, 5, 3, 4, 1, 8.25),
(13, 6, 3, 4, 1, 7.50),
(14, 7, 4, 1, 1, 6.00),
(15, 8, 4, 1, 1, 8.75),
(16, 9, 5, 3, 1, 9.25),
(17, 10, 5, 2, 1, 7.80);

-- --------------------------------------------------------

--
-- Table structure for table `students`
--

CREATE TABLE `students` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `gender` enum('Nam','Nữ','Other') DEFAULT NULL,
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
-- Dumping data for table `students`
--

INSERT INTO `students` (`id`, `fullname`, `avatar`, `birthday`, `gender`, `ethnicity`, `religion`, `address`, `phone`, `email`, `learn_year`, `learn_status`, `status`) VALUES
(1, 'Nguyễn Văn An', NULL, '2010-05-12', 'Nam', 'Kinh', 'Không', '123 Lý Thường Kiệt, TP. Hồ Chí Minh', '0912345678', 'an.nguyen@example.com', '2021-2024', 'Đang học', 1),
(2, 'Trần Thị Bình', NULL, '2010-09-23', 'Nữ', 'Kinh', 'Phật giáo', '45 Nguyễn Trãi, TP. Hồ Chí Minh', '0923456789', 'binh.tran@example.com', '2022-2025', 'Đang học', 1),
(3, 'Lê Văn Cường', NULL, '2009-03-15', 'Nam', 'Mường', 'Không', '67 Hai Bà Trưng, TP. Hồ Chí Minh', '0934567890', 'cuong.le@example.com', '2020-2023', 'Bảo lưu', 1),
(4, 'Phạm Thị Dung', NULL, '2011-07-08', 'Nữ', 'Kinh', 'Công giáo', '89 Điện Biên Phủ, TP. Hồ Chí Minh', '0945678901', 'dung.pham@example.com', '2021-2024', 'Đang học', 1),
(5, 'Hoàng Văn Em', NULL, '2011-11-30', 'Nam', 'Thái', 'Không', '12 Cách Mạng Tháng Tám, TP. Hồ Chí Minh', '0956789012', 'em.hoang@example.com', '2022-2025', 'Đang học', 1),
(6, 'Đỗ Thị Hạnh', NULL, '2008-04-22', 'Nữ', 'Kinh', 'Phật giáo', '234 Nguyễn Văn Cừ, TP. Hồ Chí Minh', '0967890123', 'hanh.do@example.com', '2019-2022', 'Tốt nghiệp', 1),
(7, 'Bùi Văn Khánh', NULL, '2010-01-19', 'Nam', 'Tày', 'Không', '56 Võ Văn Tần, TP. Hồ Chí Minh', '0978901234', 'khanh.bui@example.com', '2021-2024', 'Đang học', 1),
(8, 'Ngô Thị Lan', NULL, '2011-06-05', 'Nữ', 'Kinh', 'Không', '78 Trần Hưng Đạo, TP. Hồ Chí Minh', '0989012345', 'lan.ngo@example.com', '2022-2025', 'Nghỉ học', 1),
(9, 'Vũ Văn Minh', NULL, '2009-08-17', 'Nam', 'Hoa', 'Không', '90 Pasteur, TP. Hồ Chí Minh', '0990123456', 'minh.vu@example.com', '2020-2023', 'Bảo lưu', 1),
(10, 'Phan Thị Ngọc', NULL, '2011-12-25', 'Nữ', 'Khmer', 'Không', '321 Nguyễn Thị Minh Khai, TP. Hồ Chí Minh', '0901234567', 'ngoc.phan@example.com', '2021-2024', 'Đang học', 1),
(11, 'Nguyễn Văn A', NULL, '2007-05-12', 'Nam', 'Kinh', 'Không', '123 Lê Lợi, TP.HCM', '0901234567', 'nva@example.com', '2022-2025', 'Đang học', 1),
(12, 'Trần Thị B', NULL, '2008-03-22', 'Nữ', 'Kinh', 'Phật giáo', '456 Nguyễn Huệ, TP.HCM', '0902345678', 'ttb@example.com', '2022-2025', 'Đang học', 1),
(13, 'Lê Văn C', NULL, '2007-11-30', 'Nam', 'Kinh', 'Thiên chúa', '789 Hai Bà Trưng, TP.HCM', '0903456789', 'lvc@example.com', '2022-2025', 'Đang học', 1),
(14, 'Phạm Thị D', NULL, '2008-07-15', 'Nữ', 'Kinh', 'Không', '321 Trần Hưng Đạo, TP.HCM', '0904567890', 'ptd@example.com', '2022-2025', 'Đang học', 1),
(15, 'Hoàng Văn E', NULL, '2007-09-05', 'Nam', 'Kinh', 'Không', '654 Võ Văn Tần, TP.HCM', '0905678901', 'hve@example.com', '2022-2025', 'Đang học', 1),
(16, 'Đặng Thị F', NULL, '2008-01-18', 'Nữ', 'Kinh', 'Phật giáo', '987 Nguyễn Thị Minh Khai, TP.HCM', '0906789012', 'dtf@example.com', '2022-2025', 'Đang học', 1),
(17, 'Vũ Văn G', NULL, '2007-06-25', 'Nam', 'Kinh', 'Không', '159 Cách Mạng Tháng 8, TP.HCM', '0907890123', 'vvg@example.com', '2022-2025', 'Đang học', 1),
(18, 'Bùi Thị H', NULL, '2008-10-10', 'Nữ', 'Kinh', 'Thiên chúa', '753 Điện Biên Phủ, TP.HCM', '0908901234', 'bth@example.com', '2022-2025', 'Đang học', 1),
(19, 'Ngô Văn I', NULL, '2007-12-01', 'Nam', 'Kinh', 'Không', '852 Nguyễn Đình Chiểu, TP.HCM', '0909012345', 'nvi@example.com', '2022-2025', 'Đang học', 1),
(20, 'Dương Thị J', NULL, '2008-04-09', 'Nữ', 'Kinh', 'Phật giáo', '951 Lý Tự Trọng, TP.HCM', '0910123456', 'dtj@example.com', '2022-2025', 'Đang học', 1),
(21, 'Tạ Văn K', NULL, '2007-08-14', 'Nam', 'Kinh', 'Không', '147 Pasteur, TP.HCM', '0911234567', 'tvk@example.com', '2022-2025', 'Đang học', 1),
(22, 'Lâm Thị L', NULL, '2008-02-28', 'Nữ', 'Kinh', 'Thiên chúa', '369 Nam Kỳ Khởi Nghĩa, TP.HCM', '0912345678', 'ltl@example.com', '2022-2025', 'Đang học', 1),
(23, 'Mai Văn M', NULL, '2007-10-20', 'Nam', 'Kinh', 'Không', '258 Nguyễn Trãi, TP.HCM', '0913456789', 'mvm@example.com', '2022-2025', 'Đang học', 1),
(24, 'Thảo Thị N', NULL, '2008-06-06', 'Nữ', 'Kinh', 'Phật giáo', '147 Lý Chính Thắng, TP.HCM', '0914567890', 'ttn@example.com', '2022-2025', 'Đang học', 1),
(25, 'Quách Văn O', NULL, '2007-03-17', 'Nam', 'Kinh', 'Không', '369 Trương Định, TP.HCM', '0915678901', 'qvo@example.com', '2022-2025', 'Đang học', 1),
(26, 'Hồ Thị P', NULL, '2008-09-29', 'Nữ', 'Kinh', 'Thiên chúa', '753 Nguyễn Văn Cừ, TP.HCM', '0916789012', 'htp@example.com', '2022-2025', 'Đang học', 1),
(27, 'Lương Văn Q', NULL, '2007-01-03', 'Nam', 'Kinh', 'Không', '852 Tôn Đức Thắng, TP.HCM', '0917890123', 'lvq@example.com', '2022-2025', 'Đang học', 1),
(28, 'Phan Thị R', NULL, '2008-12-12', 'Nữ', 'Kinh', 'Phật giáo', '951 Bà Huyện Thanh Quan, TP.HCM', '0918901234', 'ptr@example.com', '2022-2025', 'Đang học', 1),
(29, 'Trịnh Văn S', NULL, '2007-07-07', 'Nam', 'Kinh', 'Không', '147 Nguyễn Phi Khanh, TP.HCM', '0919012345', 'tvs@example.com', '2022-2025', 'Đang học', 1),
(30, 'Lý Thị T', NULL, '2008-05-01', 'Nữ', 'Kinh', 'Thiên chúa', '369 Nguyễn Văn Thủ, TP.HCM', '0920123456', 'ltt@example.com', '2022-2025', 'Đang học', 1),
(31, 'Nguyễn Thị U', NULL, '2008-03-11', 'Nữ', 'Kinh', 'Không', '101 Nguyễn Văn Đậu, TP.HCM', '0921234567', 'ntu@example.com', '2022-2025', 'Đang học', 1),
(32, 'Trần Văn V', NULL, '2007-06-19', 'Nam', 'Kinh', 'Phật giáo', '202 Phan Đăng Lưu, TP.HCM', '0922345678', 'tvv@example.com', '2022-2025', 'Đang học', 1),
(33, 'Lê Thị W', NULL, '2008-09-23', 'Nữ', 'Kinh', 'Thiên chúa', '303 Hoàng Hoa Thám, TP.HCM', '0923456789', 'ltw@example.com', '2022-2025', 'Đang học', 1),
(34, 'Phạm Văn X', NULL, '2007-12-05', 'Nam', 'Kinh', 'Không', '404 Phan Xích Long, TP.HCM', '0924567890', 'pvx@example.com', '2022-2025', 'Đang học', 1),
(35, 'Hoàng Thị Y', NULL, '2008-01-17', 'Nữ', 'Kinh', 'Phật giáo', '505 Trường Sa, TP.HCM', '0925678901', 'hty@example.com', '2022-2025', 'Đang học', 1),
(36, 'Đặng Văn Z', NULL, '2007-04-28', 'Nam', 'Kinh', 'Không', '606 Hoàng Sa, TP.HCM', '0926789012', 'dvz@example.com', '2022-2025', 'Đang học', 1),
(37, 'Vũ Thị AA', NULL, '2008-07-09', 'Nữ', 'Kinh', 'Thiên chúa', '707 Nguyễn Kiệm, TP.HCM', '0927890123', 'vtaa@example.com', '2022-2025', 'Đang học', 1),
(38, 'Bùi Văn BB', NULL, '2007-10-30', 'Nam', 'Kinh', 'Không', '808 Nguyễn Oanh, TP.HCM', '0928901234', 'bvbb@example.com', '2022-2025', 'Đang học', 1),
(39, 'Ngô Thị CC', NULL, '2008-02-14', 'Nữ', 'Kinh', 'Phật giáo', '909 Quang Trung, TP.HCM', '0929012345', 'ntcc@example.com', '2022-2025', 'Đang học', 1),
(40, 'Dương Văn DD', NULL, '2007-08-21', 'Nam', 'Kinh', 'Không', '111 Phạm Văn Chiêu, TP.HCM', '0930123456', 'dvdd@example.com', '2022-2025', 'Đang học', 1),
(41, 'Tạ Thị EE', NULL, '2008-05-03', 'Nữ', 'Kinh', 'Thiên chúa', '222 Lê Đức Thọ, TP.HCM', '0931234567', 'ttee@example.com', '2022-2025', 'Đang học', 1),
(42, 'Lâm Văn FF', NULL, '2007-11-16', 'Nam', 'Kinh', 'Không', '333 Thống Nhất, TP.HCM', '0932345678', 'lvff@example.com', '2022-2025', 'Đang học', 1),
(43, 'Mai Thị GG', NULL, '2008-06-27', 'Nữ', 'Kinh', 'Phật giáo', '444 Nguyễn Văn Lượng, TP.HCM', '0933456789', 'magg@example.com', '2022-2025', 'Đang học', 1),
(44, 'Thảo Văn HH', NULL, '2007-09-08', 'Nam', 'Kinh', 'Không', '555 Lê Văn Thọ, TP.HCM', '0934567890', 'tvhh@example.com', '2022-2025', 'Đang học', 1),
(45, 'Quách Thị II', NULL, '2008-12-19', 'Nữ', 'Kinh', 'Thiên chúa', '666 Phan Huy Ích, TP.HCM', '0935678901', 'qtii@example.com', '2022-2025', 'Đang học', 1),
(46, 'Hồ Văn JJ', NULL, '2007-03-02', 'Nam', 'Kinh', 'Không', '777 Trường Chinh, TP.HCM', '0936789012', 'hvjj@example.com', '2022-2025', 'Đang học', 1),
(47, 'Lương Thị KK', NULL, '2008-10-13', 'Nữ', 'Kinh', 'Phật giáo', '888 Âu Cơ, TP.HCM', '0937890123', 'ltkk@example.com', '2022-2025', 'Đang học', 1),
(48, 'Phan Văn LL', NULL, '2007-01-25', 'Nam', 'Kinh', 'Không', '999 Lũy Bán Bích, TP.HCM', '0938901234', 'pvll@example.com', '2022-2025', 'Đang học', 1),
(49, 'Trịnh Thị MM', NULL, '2008-04-06', 'Nữ', 'Kinh', 'Thiên chúa', '121 Tân Kỳ Tân Quý, TP.HCM', '0939012345', 'ttmm@example.com', '2022-2025', 'Đang học', 1),
(50, 'Lý Văn NN', NULL, '2007-07-18', 'Nam', 'Kinh', 'Không', '232 Tây Thạnh, TP.HCM', '0940123456', 'lvnn@example.com', '2022-2025', 'Đang học', 1);

-- --------------------------------------------------------

--
-- Table structure for table `student_exams`
--

CREATE TABLE `student_exams` (
    `examinee_id` int(11),
    `student_id` int(11) NOT NULL,          -- Học sinh nào
    `exam_id` int(11) NOT NULL              -- Thuộc phòng thi nào
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `student_exams`
--

INSERT INTO `student_exams` (`examinee_id`, `student_id`, `exam_id`) VALUES
(1, 1, 1),
(2, 2, 1),
(3, 3, 2),
(4, 4, 4),
(5, 5, 2),
(6, 6, 3),
(7, 7, 4),
(8, 8, 5),
(9, 9, 4),
(10, 10, 5),
(11, 11, 5),
(12, 12, 2),
(13, 13, 1),
(14, 14, 3),
(15, 15, 1),
(16, 16, 2),
(17, 17, 3),
(18, 18, 2),
(19, 19, 3),
(20, 20, 4);

-- --------------------------------------------------------

--
-- Table structure for table `subjects`
--

CREATE TABLE `subjects` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `description` text DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `subjects`
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
-- Table structure for table `subject_term_avg`
--

CREATE TABLE `subject_term_avg` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `subject_id` int(11) NOT NULL,
  `score` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `subject_term_avg`
--

INSERT INTO `subject_term_avg` (`assign_class_id`, `student_id`, `subject_id`, `score`) VALUES
(11, 21, 1, 8.00),
(12, 22, 2, 7.50),
(13, 23, 3, 6.80),
(14, 24, 4, 9.10),
(15, 25, 5, 8.60),
(16, 26, 6, 7.90),
(17, 27, 7, 6.70),
(18, 28, 8, 8.40),
(19, 29, 9, 9.00),
(20, 30, 10, 7.85);

-- --------------------------------------------------------

--
-- Table structure for table `teachers`
--

CREATE TABLE `teachers` (
  `id` int(11) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `avatar` varchar(255) DEFAULT NULL,
  `birthday` date DEFAULT NULL,
  `gender` enum('Nam','Nữ','Other') DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `phone` varchar(10) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `teachers`
--

INSERT INTO `teachers` (`id`, `fullname`, `avatar`, `birthday`, `gender`, `address`, `phone`, `email`, `status`) VALUES
(1, 'Nguyễn Văn Hùng', NULL, '1980-03-15', 'Nam', '123 Lê Lợi, TP.HCM', '0901234567', 'nvhung@example.com', 1),
(2, 'Trần Thị Mai', NULL, '1982-07-22', 'Nữ', '456 Nguyễn Huệ, TP.HCM', '0902345678', 'ttmai@example.com', 1),
(3, 'Lê Văn Phúc', NULL, '1979-11-30', 'Nam', '789 Hai Bà Trưng, TP.HCM', '0903456789', 'lvphuc@example.com', 1),
(4, 'Phạm Thị Hương', NULL, '1985-05-10', 'Nữ', '321 Trần Hưng Đạo, TP.HCM', '0904567890', 'pthuong@example.com', 1),
(5, 'Hoàng Văn Tâm', NULL, '1981-09-05', 'Nam', '654 Võ Văn Tần, TP.HCM', '0905678901', 'hvtam@example.com', 1),
(6, 'Đặng Thị Lan', NULL, '1983-01-18', 'Nữ', '987 Nguyễn Thị Minh Khai, TP.HCM', '0906789012', 'dtlan@example.com', 1),
(7, 'Vũ Văn Quang', NULL, '1978-06-25', 'Nam', '159 Cách Mạng Tháng 8, TP.HCM', '0907890123', 'vvquang@example.com', 1),
(8, 'Bùi Thị Ngọc', NULL, '1984-10-10', 'Nữ', '753 Điện Biên Phủ, TP.HCM', '0908901234', 'btngoc@example.com', 1),
(9, 'Ngô Văn Sơn', NULL, '1980-12-01', 'Nam', '852 Nguyễn Đình Chiểu, TP.HCM', '0909012345', 'nvson@example.com', 1),
(10, 'Dương Thị Thu', NULL, '1986-04-09', 'Nữ', '951 Lý Tự Trọng, TP.HCM', '0910123456', 'dtthu@example.com', 1);

-- --------------------------------------------------------

--
-- Table structure for table `terms`
--

CREATE TABLE `terms` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `year` varchar(50) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `end_date` date DEFAULT NULL,
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `terms`
--

INSERT INTO `terms` (`id`, `name`, `year`, `start_date`, `end_date`, `status`) VALUES
(1, 'Học kỳ 1', '2021-2022', '2021-09-01', '2022-01-15', 1),
(2, 'Học kỳ 2', '2021-2022', '2022-02-01', '2022-06-15', 1),
(3, 'Học kỳ 1', '2022-2023', '2022-09-01', '2023-01-15', 1),
(4, 'Học kỳ 2', '2022-2023', '2023-02-01', '2023-06-15', 1),
(5, 'Học kỳ 1', '2023-2024', '2023-09-01', '2024-01-15', 1),
(6, 'Học kỳ 2', '2023-2024', '2024-02-01', '2024-06-15', 1),
(7, 'Học kỳ 1', '2024-2025', '2024-09-01', '2025-01-15', 1),
(8, 'Học kỳ 2', '2024-2025', '2025-02-01', '2025-06-15', 1),
(9, 'Học kỳ 1', '2025-2026', '2025-09-01', '2026-01-15', 1),
(10, 'Học kỳ 2', '2025-2026', '2026-02-01', '2026-06-15', 1);

-- --------------------------------------------------------

--
-- Table structure for table `term_gpa`
--

CREATE TABLE `term_gpa` (
  `assign_class_id` int(11) NOT NULL,
  `student_id` int(11) NOT NULL,
  `gpa` decimal(10,2) DEFAULT NULL,
  `conduct_level` enum('Good','Fair','Satisfactory','Unsatisfactory') DEFAULT NULL,
  `academic` enum('Good','Fair','Satisfactory','Unsatisfactory') DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `term_gpa`
--

INSERT INTO `term_gpa` (`assign_class_id`, `student_id`, `gpa`, `conduct_level`, `academic`) VALUES
(11, 21, 8.25, 'Good', 'Good'),
(12, 22, 7.80, 'Fair', 'Good'),
(13, 23, 6.75, 'Satisfactory', 'Fair'),
(14, 24, 9.10, 'Good', 'Good'),
(15, 25, 8.60, 'Good', 'Good'),
(16, 26, 7.90, 'Fair', 'Fair'),
(17, 27, 6.70, 'Satisfactory', 'Satisfactory'),
(18, 28, 8.40, 'Good', 'Good'),
(19, 29, 9.00, 'Good', 'Good'),
(20, 30, 7.85, 'Fair', 'Good');




--
-- Table structure for table `users`
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
  `status` tinyint(4) DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `role_id`, `username`, `password`, `fullname`, `avatar`, `phone`, `email`, `address`, `status`) VALUES
(1, 1, 'admin01', '123456', 'Nguyễn Văn Quản Trị', NULL, '0909123456', 'admin@example.com', '232 hưng phú phường chợ lớn Tp.HCM', 1),
(2, 2, 'gv01', '123456', 'Trần Thị Giáo Viên', NULL, '0909234567', 'giaovien@example.com', '232 hưng phú phường chợ lớn Tp.HCM', 1);

-- --------------------------------------------------------

--
-- Table structure for table `violations`
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
-- Dumping data for table `violations`
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
-- Indexes for dumped tables
--

--
-- Indexes for table `assign_classes`
--
ALTER TABLE `assign_classes`
  ADD PRIMARY KEY (`id`,`class_id`,`term_id`,`head_teacher_id`),
  ADD KEY `class_id` (`class_id`),
  ADD KEY `head_teacher_id` (`head_teacher_id`),
  ADD KEY `term_id` (`term_id`);

--
-- Indexes for table `assign_class_students`
--
ALTER TABLE `assign_class_students`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `assign_class_teachers`
--
ALTER TABLE `assign_class_teachers`
  ADD PRIMARY KEY (`assign_class_id`,`teacher_id`,`subject_id`),
  ADD KEY `teacher_id` (`teacher_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Indexes for table `classes`
--
ALTER TABLE `classes`
  ADD PRIMARY KEY (`id`,`class_type_id`),
  ADD KEY `class_type_id` (`class_type_id`);

--
-- Indexes for table `class_types`
--
ALTER TABLE `class_types`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `departments`
--
ALTER TABLE `departments`
  ADD PRIMARY KEY (`id`,`subject_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Indexes for table `department_details`
--
ALTER TABLE `department_details`
  ADD PRIMARY KEY (`department_id`,`teacher_id`),
  ADD KEY `teacher_id` (`teacher_id`);

--
-- Indexes for table `exams`
--
ALTER TABLE `exams`
  ADD PRIMARY KEY (`id`,`exam_detail_id`),
  ADD KEY `exam_detail_id` (`exam_detail_id`);

--
-- Indexes for table `rooms`
--
ALTER TABLE `rooms`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `exam_details`
--
ALTER TABLE `exam_details`
  ADD PRIMARY KEY (`id`,`subject_id`,`term_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `term_id` (`term_id`);

--
-- Indexes for table `exam_types`
--
ALTER TABLE `exam_types`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `functions`
--
ALTER TABLE `functions`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `relations`
--
ALTER TABLE `relations`
  ADD PRIMARY KEY (`id`,`student_id`),
  ADD KEY `student_id` (`student_id`);

--
-- Indexes for table `roles`
--
ALTER TABLE `roles`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `role_details`
--
ALTER TABLE `role_details`
  ADD PRIMARY KEY (`role_id`,`function_id`,`action`),
  ADD KEY `function_id` (`function_id`);

--
-- Indexes for table `rules`
--
ALTER TABLE `rules`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `score_details`
--
ALTER TABLE `score_details`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`,`subject_id`,`exam_type_id`,`attempt`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `exam_type_id` (`exam_type_id`);

--
-- Indexes for table `students`
--
ALTER TABLE `students`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `student_exams`
--
ALTER TABLE `student_exams`
  ADD PRIMARY KEY (`examinee_id`,`student_id`,`exam_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `exam_id` (`exam_id`);

--
-- Indexes for table `subjects`
--
ALTER TABLE `subjects`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `subject_term_avg`
--
ALTER TABLE `subject_term_avg`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`,`subject_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `subject_id` (`subject_id`);

--
-- Indexes for table `teachers`
--
ALTER TABLE `teachers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `terms`
--
ALTER TABLE `terms`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `term_gpa`
--
ALTER TABLE `term_gpa`
  ADD PRIMARY KEY (`assign_class_id`,`student_id`),
  ADD KEY `student_id` (`student_id`);


ALTER TABLE `users`
  ADD PRIMARY KEY (`id`,`role_id`),
  ADD UNIQUE KEY `username` (`username`),
  ADD KEY `role_id` (`role_id`);

--
-- Indexes for table `violations`
--
ALTER TABLE `violations`
  ADD PRIMARY KEY (`id`,`student_id`,`assign_class_id`),
  ADD KEY `student_id` (`student_id`),
  ADD KEY `assign_class_id` (`assign_class_id`),
  ADD KEY `rule_id` (`rule_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `assign_classes`
--
ALTER TABLE `assign_classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `classes`
--
ALTER TABLE `classes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `class_types`
--
ALTER TABLE `class_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `departments`
--
ALTER TABLE `departments`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `exams`
--
ALTER TABLE `exams`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `exam_details`
--
ALTER TABLE `exam_details`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `exam_types`
--
ALTER TABLE `exam_types`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `functions`
--
ALTER TABLE `functions`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `relations`
--
ALTER TABLE `relations`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `roles`
--
ALTER TABLE `roles`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `rules`
--
ALTER TABLE `rules`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `students`
--
ALTER TABLE `students`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `student_exams`
--
ALTER TABLE `student_exams`
  MODIFY `examinee_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=21;

--
-- AUTO_INCREMENT for table `subjects`
--
ALTER TABLE `subjects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `teachers`
--
ALTER TABLE `teachers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `terms`
--
ALTER TABLE `terms`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `violations`
--
ALTER TABLE `violations`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;





--
-- Constraints for dumped tables
--

--
-- Constraints for table `assign_classes`
--
ALTER TABLE `assign_classes`
  ADD CONSTRAINT `assign_classes_ibfk_1` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`),
  ADD CONSTRAINT `assign_classes_ibfk_2` FOREIGN KEY (`head_teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `assign_classes_ibfk_3` FOREIGN KEY (`term_id`) REFERENCES `terms` (`id`);

--
-- Constraints for table `assign_class_students`
--
ALTER TABLE `assign_class_students`
  ADD CONSTRAINT `assign_class_students_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `assign_class_students_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `assign_class_teachers`
--
ALTER TABLE `assign_class_teachers`
  ADD CONSTRAINT `assign_class_teachers_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `assign_class_teachers_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`),
  ADD CONSTRAINT `assign_class_teachers_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Constraints for table `classes`
--
ALTER TABLE `classes`
  ADD CONSTRAINT `classes_ibfk_1` FOREIGN KEY (`class_type_id`) REFERENCES `class_types` (`id`);

--
-- Constraints for table `departments`
--
ALTER TABLE `departments`
  ADD CONSTRAINT `departments_ibfk_1` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Constraints for table `department_details`
--
ALTER TABLE `department_details`
  ADD CONSTRAINT `department_details_ibfk_1` FOREIGN KEY (`department_id`) REFERENCES `departments` (`id`),
  ADD CONSTRAINT `department_details_ibfk_2` FOREIGN KEY (`teacher_id`) REFERENCES `teachers` (`id`);

--
-- Constraints for table `exams`
--
ALTER TABLE `exams`
  ADD CONSTRAINT `exams_ibfk_1` FOREIGN KEY (`exam_detail_id`) REFERENCES `exam_details`(`id`),
  ADD CONSTRAINT `exams_ibfk_2` FOREIGN KEY (`supervisor_id`) REFERENCES `teachers`(`id`),
  ADD CONSTRAINT `exams_ibfk_3` FOREIGN KEY (`exam_room`) REFERENCES `rooms`(`id`);

--
-- Constraints for table `exam_details`
--
ALTER TABLE `exam_details`
  ADD CONSTRAINT `exam_details_ibfk_1` FOREIGN KEY (`subject_id`) REFERENCES `subjects`(`id`),
  ADD CONSTRAINT `exam_details_ibfk_2` FOREIGN KEY (`term_id`) REFERENCES `terms`(`id`),
  ADD CONSTRAINT `exam_details_ibfk_3` FOREIGN KEY (`exam_type_id`) REFERENCES `exam_types`(`id`);

--
-- Constraints for table `relations`
--
ALTER TABLE `relations`
  ADD CONSTRAINT `relations_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `role_details`
--
ALTER TABLE `role_details`
  ADD CONSTRAINT `role_details_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`),
  ADD CONSTRAINT `role_details_ibfk_2` FOREIGN KEY (`function_id`) REFERENCES `functions` (`id`);

--
-- Constraints for table `score_details`
--
ALTER TABLE `score_details`
  ADD CONSTRAINT `score_details_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `score_details_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `score_details_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`),
  ADD CONSTRAINT `score_details_ibfk_4` FOREIGN KEY (`exam_type_id`) REFERENCES `exam_types` (`id`);

--
-- Constraints for table `student_exams`
--
ALTER TABLE `student_exams`
  ADD CONSTRAINT `student_exams_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `student_exams_ibfk_2` FOREIGN KEY (`exam_id`) REFERENCES `exams` (`id`);

--
-- Constraints for table `subject_term_avg`
--
ALTER TABLE `subject_term_avg`
  ADD CONSTRAINT `subject_term_avg_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `subject_term_avg_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `subject_term_avg_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`);

--
-- Constraints for table `term_gpa`
--
ALTER TABLE `term_gpa`
  ADD CONSTRAINT `term_gpa_ibfk_1` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `term_gpa_ibfk_2` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`);

--
-- Constraints for table `tuitions`
--

--

ALTER TABLE `users`
  ADD CONSTRAINT `users_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`);

--
-- Constraints for table `violations`
--
ALTER TABLE `violations`
  ADD CONSTRAINT `violations_ibfk_1` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`),
  ADD CONSTRAINT `violations_ibfk_2` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
  ADD CONSTRAINT `violations_ibfk_3` FOREIGN KEY (`rule_id`) REFERENCES `rules` (`id`);
COMMIT;




-- 1) Months: danh sách các tháng (dùng chung)
CREATE TABLE `months` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL COMMENT 'Ví dụ: Tháng 8, Tháng 9, ...',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


-- 2) Fee templates: định nghĩa loại phí
CREATE TABLE `fee_templates` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(200) NOT NULL COMMENT 'Tên phí: Học phí, Tiền xe, Bữa ăn, ...',
  `description` TEXT NULL,
  `fee_type` ENUM('BASE','EXTRA') NOT NULL DEFAULT 'BASE' COMMENT 'BASE = phí cơ bản, EXTRA = phí phát sinh/khác',
  `default amount` DECIMAL(12,2) NOT NULL DEFAULT 0,
  `is_active` TINYINT(1) NOT NULL DEFAULT 1,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `idx_fee_type` (`fee_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;
-- CREATE TABLE `class_fee_templates` (
--     `id` int(11) NOT NULL AUTO_INCREMENT,
--     `assign_class_id` int(11) NOT NULL,      -- Liên kết với assign_classes
--     `fee_template_id` int(11) NOT NULL,      -- Liên kết với fee_templates
--     `start_date` date NOT NULL,
--     `end_date` date NOT NULL,
--     `is_active` tinyint(1) NOT NULL DEFAULT 1,
--     PRIMARY KEY (`id`),
--     KEY `idx_assign_class` (`assign_class_id`),
--     KEY `idx_fee_template` (`fee_template_id`),
--     CONSTRAINT `fk_class_fee_assign` FOREIGN KEY (`assign_class_id`) REFERENCES `assign_classes` (`id`),
--     CONSTRAINT `fk_class_fee_template` FOREIGN KEY (`fee_template_id`) REFERENCES `fee_templates` (`id`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


CREATE TABLE `class_fee_months` (
  `id` INT NOT NULL AUTO_INCREMENT,

  -- Ánh xạ lớp học kỳ
  `assign_class_id` INT NOT NULL,

  -- Loại phí
  `fee_template_id` INT NOT NULL,

  -- Tháng
  `month_id` INT NOT NULL,

  -- Học kỳ
  `term` TINYINT(1) NOT NULL COMMENT '1 = HK1, 2 = HK2',

  -- Bật/tắt tháng đó
  `is_selected` TINYINT(1) NOT NULL DEFAULT 1,

  -- Số tiền tháng đó
  `amount` DECIMAL(12,2) NOT NULL DEFAULT 0,

  -- Thời gian áp dụng chung cho phí (có thể NULL)
  `start_date` DATE DEFAULT NULL,
  `end_date` DATE DEFAULT NULL,

  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (`id`),

  KEY `idx_class_fee_months_assign` (`assign_class_id`),
  KEY `idx_class_fee_months_template` (`fee_template_id`),
  KEY `idx_class_fee_months_month` (`month_id`),

  CONSTRAINT `fk_cfm_assign_class` FOREIGN KEY (`assign_class_id`)
       REFERENCES `assign_classes` (`id`) ON DELETE CASCADE,

  CONSTRAINT `fk_cfm_fee_template` FOREIGN KEY (`fee_template_id`)
       REFERENCES `fee_templates` (`id`) ON DELETE CASCADE,

  CONSTRAINT `fk_cfm_month` FOREIGN KEY (`month_id`)
       REFERENCES `months` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


=


-- 12) TuitionPayment: các lần thanh toán (ghi log)
CREATE TABLE tuition_monthly (
    id INT AUTO_INCREMENT PRIMARY KEY,

    student_id INT NOT NULL,
    assign_class_id INT NOT NULL,
    month_id INT NOT NULL,

    total_amount DECIMAL(12,2) NOT NULL DEFAULT 0,

    is_paid TINYINT(1) NOT NULL DEFAULT 0, -- 0 = chưa đóng, 1 = đã đóng

    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    CONSTRAINT fk_tm_student
        FOREIGN KEY (student_id) REFERENCES students(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_tm_assign_class
        FOREIGN KEY (assign_class_id) REFERENCES assign_class_students(assign_class_id)
        ON DELETE CASCADE,

    CONSTRAINT fk_tm_month
        FOREIGN KEY (month_id) REFERENCES months(id)
        ON DELETE CASCADE
);



INSERT INTO months (id, name) VALUES
(1, 'Tháng 1'),
(2, 'Tháng 2'),
(3, 'Tháng 3'),
(4, 'Tháng 4'),
(5, 'Tháng 5'),
(6, 'Tháng 6'),
(7, 'Tháng 7'),
(8, 'Tháng 8'),
(9, 'Tháng 9'),
(10, 'Tháng 10'),
(11, 'Tháng 11'),
(12, 'Tháng 12');



SELECT 
    tm.id,
    tm.student_id,
    s.fullname AS name,
    ac.class_id,
    c.name AS class_name,
    c.grade,
    c.year AS class_year,

    tm.month_id,
    m.name AS month_name,

    tm.amount,
    tm.paid_amount,
    tm.status
FROM tuition_monthly tm
JOIN students s ON tm.student_id = s.id
LEFT JOIN assign_classes ac 
       ON ac.student_id = tm.student_id
      AND ac.year = tm.year
LEFT JOIN classes c ON c.id = ac.class_id
LEFT JOIN months m ON m.id = tm.month_id
WHERE tm.year = @year;


