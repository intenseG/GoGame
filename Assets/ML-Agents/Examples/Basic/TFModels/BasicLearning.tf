
?
is_continuous_controlConst*
value	B : *
dtype0
8
version_numberConst*
value	B :*
dtype0
5
memory_sizeConst*
value	B : *
dtype0
=
action_output_shapeConst*
dtype0*
value	B :
L
vector_observationPlaceholder*
dtype0*
shape:���������
�
main_graph_0/hidden_0/kernelConst*�
value�B�"��bQ>ngy�<�6���������'m�����q�2>��>KL�7I�cս��k�m>��=��\�>��}=
*<���\����;��->r>Q>;G��x,�Q@�>���=&Ai<�����:��h]���?Q���>hM9>��=��>5xh=�B��ב��k���1ؽд������P�<�O'>�{��/O����=l��>�3���<.~�>�3Ժ��˾��Os���F ?wcK>�g�=��
�m����=aq>TB�>=R�>NX!�=+�>��>�K>���=j
>���>��>C/�k`��\F� �F���>��n>���>Sw�=��>[��=�q��N�-=
�Ծ�#f���>-M>L"�>G>C��C�=�96�/Ȇ�F��ԣ_���f>���>�C���L>������>bʰ��s�>�$¾�W>�_����G��<��3�`�E��̢=]i ��:�=�e4��Q�F�>��r�?������'�^�[ש>2�I��H��.�>bD�=�"��x��iX���>�(��˟��U�=�Ե��Zǽ�����>W,<��>)ط>Y�����>�̘=ft�=�喾AČ�G�4>M����V�=Z�K>�;�1ۧ=����>h���s�R�w�.��=�M�=Ps>(X�<��ڽs�>�P��-�>m�#�,��>o�U�0h>I��x�>��ľ��>��n�x�>}h�>��!��?�ԾW�=�|�>]��	k�=W����=T��>ق�>�Y,=}��>��>� �=��$�(>$�k>�ýȽ=��=���>��<�T�B>v֕=���=T��>�a>�K���>�S�>ab��Ҿ���=&U�7�?�)���[�jǘ���A����=�o=p�?|+#?�C=�9?b??~���K*��(<?��� �?� ;�A>&�:=�����@$�pϞ>�G>�-����Ƚ����r!??P?�O>���>W�?�eپ$�ž,��>�!<�)��>���KP���!��@+S?������Q>�Cj�����H	?�`��� ?�,?�\8?�*�>D��>wL�/��7̯> �ؾn�?댋��;Q�̝���9#?'%j�*�0>	N>�n���?�s��?���=-�?�@�>7��=X�������g>ə
�4_�=e`K��ؔ�Q����$?���9+�>�๾�4w��ߛ>�;ǽ���>h4? �>�
�>{�`>b��<j󾳼�>RV��6>����L�پ�����>b �?Y"?
�����>�}�>?�Ͼ��|>+��>I?4+?(o�>gr��r
���>��
�χ^?�Ӿ������9���>E������>m2��޶���J<������>M�='�>@��=�@�=^���8W��E����2�<�L<�����k>��M�=�=a�)>��@>���{ྗ�=�ܦ�A��>C�)��>�z�=1���2W��ƽ�D(����<��ʻ4t�=m_f=c]�>.�>�+[�#����>ھy��&D�v��>[�>�D>�A� �>�w>2�;>�Ã��̾�xԾ[�w����=��7>�T۾/�׽q��U��>l�׽�:�>B:E�6���*
dtype0
�
!main_graph_0/hidden_0/kernel/readIdentitymain_graph_0/hidden_0/kernel*
T0*/
_class%
#!loc:@main_graph_0/hidden_0/kernel
�
main_graph_0/hidden_0/biasConst*e
value\BZ"P�T?^��>��?�X ?�޾�IȾ/?pܾ��
?K�þW�����t?ZSɾ:��>L��f0ƾ���>��{�	?*
dtype0

main_graph_0/hidden_0/bias/readIdentitymain_graph_0/hidden_0/bias*
T0*-
_class#
!loc:@main_graph_0/hidden_0/bias
�
main_graph_0/hidden_0/MatMulMatMulvector_observation!main_graph_0/hidden_0/kernel/read*
T0*
transpose_a( *
transpose_b( 
�
main_graph_0/hidden_0/BiasAddBiasAddmain_graph_0/hidden_0/MatMulmain_graph_0/hidden_0/bias/read*
data_formatNHWC*
T0
P
main_graph_0/hidden_0/SigmoidSigmoidmain_graph_0/hidden_0/BiasAdd*
T0
g
main_graph_0/hidden_0/MulMulmain_graph_0/hidden_0/BiasAddmain_graph_0/hidden_0/Sigmoid*
T0
�
dense/kernelConst*�
value�B�"��u��kھ{!?�ڪ�XǾl�>����߾�E�>��Ӿ����C&�>�߽>�t�>��ؾfL�>
��>c/����<ھA1
?���>���>�߾�P��E(ݾ���>�Z�>�t�>Wn޾5۔>;�>aJƾ�Y�>���>im޾S#پ�ξ �>`�>��>O�ξ�����j��?	�b>٤�>�&����>�2�>9�ؾ�����wɾY��>Ň>��>�n�gc�L����?*
dtype0
U
dense/kernel/readIdentitydense/kernel*
T0*
_class
loc:@dense/kernel
s
dense/MatMulMatMulmain_graph_0/hidden_0/Muldense/kernel/read*
transpose_b( *
T0*
transpose_a( 
/
action_probsIdentitydense/MatMul*
T0
F
action_masksPlaceholder*
dtype0*
shape:���������
H
strided_slice/stackConst*
valueB"        *
dtype0
J
strided_slice/stack_1Const*
valueB"       *
dtype0
J
strided_slice/stack_2Const*
dtype0*
valueB"      
�
strided_sliceStridedSliceaction_probsstrided_slice/stackstrided_slice/stack_1strided_slice/stack_2*
shrink_axis_mask *

begin_mask*
ellipsis_mask *
new_axis_mask *
end_mask*
Index0*
T0
J
strided_slice_1/stackConst*
valueB"        *
dtype0
L
strided_slice_1/stack_1Const*
valueB"       *
dtype0
L
strided_slice_1/stack_2Const*
valueB"      *
dtype0
�
strided_slice_1StridedSliceaction_masksstrided_slice_1/stackstrided_slice_1/stack_1strided_slice_1/stack_2*
shrink_axis_mask *

begin_mask*
ellipsis_mask *
new_axis_mask *
end_mask*
Index0*
T0
*
SoftmaxSoftmaxstrided_slice*
T0
-
MulMulSoftmaxstrided_slice_1*
T0
2
add/yConst*
valueB
 *���.*
dtype0

addAddMuladd/y*
T0
4
add_1/yConst*
valueB
 *���.*
dtype0
#
add_1Addaddadd_1/y*
T0
?
Sum/reduction_indicesConst*
dtype0*
value	B :
N
SumSumadd_1Sum/reduction_indices*
T0*

Tidx0*
	keep_dims(
%
truedivRealDivaddSum*
T0

Log_1Logtruediv*
T0
$
concat_1IdentityLog_1*
T0
%
actionIdentityconcat_1*
T0
�
dense_1/kernelConst*i
value`B^"P:>o��>�M>�7�>Ξ>��þ�̾�ս9u:�h����_��^�Ơ�>�HG>Gy?�c�[�ھ!a�������*
dtype0
[
dense_1/kernel/readIdentitydense_1/kernel*
T0*!
_class
loc:@dense_1/kernel
=
dense_1/biasConst*
valueB*ƍ�<*
dtype0
U
dense_1/bias/readIdentitydense_1/bias*
T0*
_class
loc:@dense_1/bias
w
dense_1/MatMulMatMulmain_graph_0/hidden_0/Muldense_1/kernel/read*
transpose_a( *
transpose_b( *
T0
]
dense_1/BiasAddBiasAdddense_1/MatMuldense_1/bias/read*
T0*
data_formatNHWC
4
value_estimateIdentitydense_1/BiasAdd*
T0 